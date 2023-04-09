using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    float m_Horizontal = 0.0f;
    [SerializeField][Range(0, 120)] int m_Degree;
    [SerializeField][Range(1, 10)] private int m_Speed;
    [SerializeField] private bool m_MouseBasedRotation;
    
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (m_MouseBasedRotation)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = transform.position.z;
            mousePos -= Camera.main.WorldToScreenPoint(transform.position);
            float angle = Mathf.Atan2(mousePos.x, mousePos.y) * Mathf.Rad2Deg;
            angle = Mathf.Clamp(angle, -m_Degree, m_Degree);
            transform.rotation = Quaternion.Euler(0, 0, -angle);
        }
        else
        {
            float horizontalDelta = Input.GetAxis("Horizontal");
            m_Horizontal += horizontalDelta * m_Speed * Time.deltaTime;
            m_Horizontal = Mathf.Clamp(m_Horizontal, -m_Degree, m_Degree);
            transform.rotation = Quaternion.Euler(0, 0, -m_Horizontal);
        }
    }

    public void SetMouseBasedRotation(bool b) => m_MouseBasedRotation = b;
}

