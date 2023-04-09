using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    //Transform LocalFireTransform;
    [SerializeField] Transform LocalFireDirection;
    bool m_CanFire;
    float m_TimeToNextShot;
    float fireFrequency = 0.25f;
    [SerializeField] private bool m_AimAssist;
    [SerializeField] ProjectileSpawner m_Spawner;

    [SerializeField] MouseEffect m_MouseEffect;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(m_AimAssist)
            Debug.DrawLine(transform.position, LocalFireDirection.position);

        if (!m_CanFire)
        {
            m_TimeToNextShot += Time.fixedDeltaTime;

            if (m_TimeToNextShot >= fireFrequency)
            {
                m_CanFire = true;
                m_TimeToNextShot = 0.0f;
            }
        }

        if (gameObject.transform.childCount == 1 &&
            m_Spawner.transform.childCount == 1)
        {
            GameObject projectile = m_Spawner.transform.GetChild(0).gameObject;
            m_MouseEffect.SetColor(m_Spawner.transform.GetChild(0).gameObject.GetComponent<Projectile>().GetColor());
            projectile.transform.parent = transform;
            projectile.transform.position = transform.position;
        }

        if (m_CanFire && Input.GetKey(KeyCode.Space))// || Input.GetMouseButtonDown(0))
        {
            m_CanFire = false;

            Vector3 firePos = transform.position;
            Vector3 fireDir = -GetFireDirection();

            fireDir.Normalize();

            GameObject projectile = transform.GetChild(1).gameObject;
            projectile.transform.parent = null;
            projectile.GetComponent<Projectile>().Init(transform.position, fireDir);
        }
    }

    Vector3 GetFireDirection()
    {
        return transform.position - LocalFireDirection.transform.position;
    }
}