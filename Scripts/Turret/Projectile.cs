using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class Projectile : MonoBehaviour
{
    public string SortingLayerName = "Default";

    Vector3 m_Velocity;
    [SerializeField] float m_Speed = 0.1f;
    CubeGrid m_CubeGrid;
    private Color m_Color;

    private bool m_FirstEnter = true;

    [SerializeField] VisualEffect m_VFX;
    [SerializeField] Gradient m_Gradient;
    [SerializeField] MouseEffect m_MouseEffect;

    // Start is called before the first frame update
    void Start()
    {
        m_CubeGrid = FindObjectOfType<CubeGrid>();
        m_FirstEnter = true;

        if (m_VFX)
        {
            m_Gradient = m_VFX.GetGradient("ColorGradiant");
        }

        GetComponent<Renderer>().sortingLayerName = "Sphere";
        GetComponent<Renderer>().sortingOrder = 10;
    }

    public void Init(Vector3 position, Vector3 velocity)
    {
        transform.position = position;
        m_Velocity = velocity;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += m_Velocity * m_Speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Cube")
        {
            if(m_FirstEnter)
            {
                m_FirstEnter = false;
                m_CubeGrid.EnableCube(coll.gameObject.GetComponent<ClickableCube>(), m_Color);
            }
            Destroy(gameObject);
        }
    }
    
    public void SetColor(Color color)
    {
        m_Color = color;
        GetComponent<Renderer>().material.color = m_Color;
        if (m_VFX)
        {
            Gradient newGradient = new Gradient();
            newGradient.mode = m_Gradient.mode;
            newGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(m_Color, 0f), new GradientColorKey(m_Color, 1f) },
                m_Gradient.alphaKeys
            );
            m_VFX.SetGradient("ColorGradiant", newGradient);
        }
    }

    public Color GetColor()
    {
        return m_Color;
    }
}
