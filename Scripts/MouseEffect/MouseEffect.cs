using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MouseEffect : MonoBehaviour
{
    [SerializeField] private Transform m_TurretPos;
    // Speed at which the effect moves towards the mouse position
    public float speed = 10f;
    [SerializeField][Range(-100.0f, 100.0f)] private float m_PosZ = 0.0f;

    private Color m_Color;
    [SerializeField] VisualEffect m_VFX;
    [SerializeField] TrailRenderer m_Trail;
    [SerializeField] Gradient m_Gradient;

    // Start is called before the first frame update
    //void Start()
    //{
    //    Cursor.visible = false;
    //}

    //void OnDestroy()
    //{
    //    Cursor.visible = true;
    //}

    // Update is called once per frame
    void Update()
    {
        // Get the current mouse position in screen coordinates
        Vector3 mousePos = Input.mousePosition;
        // Convert the screen coordinates to world coordinates
        mousePos.z = 1.0f; // Distance from the camera
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        // Move the effect towards the mouse position
        transform.position = mousePos; //Vector3.Lerp(transform.position, mousePos, speed * Time.deltaTime);
    }

    public void SetColor(Color color)
    {
        m_Color = color;
        //GetComponent<Renderer>().material.color = m_Color;
        if (m_VFX)
        {
            Gradient newGradient = new Gradient();
            newGradient.mode = m_Gradient.mode;
            newGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(m_Color, 0f), new GradientColorKey(m_Color, 1f) },
                m_Gradient.alphaKeys
            );
            m_VFX.SetGradient("ColorGradiant", newGradient);

            m_Trail.material.SetColor("_Color", m_Color);
            m_Trail.material.SetColor("_Color_1", Color.white);
        }
    }
}
