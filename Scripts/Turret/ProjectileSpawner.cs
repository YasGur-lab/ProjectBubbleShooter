using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{

    [SerializeField] GameObject m_Projectile;
    [SerializeField] CubeGrid m_CubeGrid;

    [SerializeField][Range(0, 1.0f)] float m_Timer = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        m_CubeGrid = FindObjectOfType<CubeGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Timer <= 0.0f)
        {
            if (gameObject.transform.childCount == 0)
            {
                GameObject projectile;
                projectile = Instantiate(m_Projectile, transform);
                projectile.GetComponent<Projectile>().SetColor(m_CubeGrid.GetClusterColor());
            }
        }
        else
        {
            m_Timer -= Time.deltaTime;
        }
    }
}
