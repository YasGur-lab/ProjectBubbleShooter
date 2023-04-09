using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class ClickableCube : MonoBehaviour
{
    public string SortingLayerName = "Default";

    [SerializeField] private bool m_IsProjectile;
    [SerializeField] [Range(-5.0f, 5.0f)] private float m_Intensity = 0.0f;

    [SerializeField] ExposedProperty m_MyProperty;
    [SerializeField] VisualEffect m_VFX;
    [SerializeField] Gradient m_Gradient;
    [SerializeField] Vector3 m_DirectionAfterFalling;
    [SerializeField] float m_DirectionSpeed;
    void Start()
    {

        //Choose randomly if the cube should start activated or not
        Activated = true;
        m_MyProperty = "ColorGradiant"; // Assign A string
        UpdateVisuals();

        if (m_VFX)
            m_Gradient = m_VFX.GetGradient("ColorGradiant");

        GetComponent<Renderer>().sortingLayerName = "Sphere";
        GetComponent<Renderer>().sortingOrder = 10;
    }

    void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddForce(m_DirectionAfterFalling * m_DirectionSpeed);
    }

    public bool Activated
    {
        get
        {
            return m_Activated;
        }

        set
        {
            m_Activated = value;

            //Update the visuals since the activated value changes
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        if (GetComponent<Renderer>().material == null)
        {
            return;
        }

        //Set the material color 
        if (m_Activated)
        {
            //GetComponent<Renderer>().material.color = m_ActivatedColor;
            //if (m_VFX)
            //{
            //    m_Gradient.colorKeys[0].color = m_ActivatedColor;
            //    m_Gradient.colorKeys[1].color = m_ActivatedColor;
            //    m_VFX.SetGradient("ColorGradiant", m_Gradient);
            //}

            GetComponent<Renderer>().material.color = m_ActivatedColor;
            if (m_VFX)
            {
                Gradient newGradient = new Gradient();
                newGradient.mode = m_Gradient.mode;
                newGradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(m_ActivatedColor, 0f), new GradientColorKey(m_ActivatedColor, 1f) },
                    m_Gradient.alphaKeys
                );
                m_VFX.SetGradient("ColorGradiant", newGradient);
            }
        }
    }

    public void SetColor(Color color)
    {
        m_ActivatedColor = color;
        GetComponent<Renderer>().material.SetColor("_EmissionColor", m_ActivatedColor * 100.0f);
        GetComponent<Renderer>().material.SetColor("_Color", m_ActivatedColor * 100.0f);
        if (m_VFX)
        {
            Gradient newGradient = new Gradient();
            newGradient.mode = m_Gradient.mode;
            newGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(m_ActivatedColor, 0f), new GradientColorKey(m_ActivatedColor, 1f) },
                m_Gradient.alphaKeys
            );
            m_VFX.SetGradient("ColorGradiant", newGradient);
        }
    }

    public IntVector2 m_Index;
    public Color GetColor() { return m_ActivatedColor;}

    private bool m_Activated;
    private Color m_ActivatedColor;
}
