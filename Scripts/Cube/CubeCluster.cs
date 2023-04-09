using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CubeCluster
{
    List<ClickableCube> m_Cubes;
    private bool m_HasAlreadyFallen;

    public CubeCluster()
    {
        m_Cubes = new List<ClickableCube>();
        
        if (GameObject.Find("CubeGrid").GetComponent<CubeGrid>())
        {
            GameObject.Find("CubeGrid").GetComponent<CubeGrid>().CalculatedCluster = true;
        }
    }

    public void AddCube(ClickableCube cube)
    {
        m_Cubes.Add(cube);
    }

    public int GetNumOfCubes()
    {
        return m_Cubes.Count;
    }

    public void DeactivateCube()
    {
        for (int i = 0; i < m_Cubes.Count; i++)
        {
            m_Cubes[i].Activated = false;
            m_Cubes[i].Activated = false;

            m_Cubes[i].GetComponent<Rigidbody>().isKinematic = false;
            m_Cubes[i].GetComponent<Rigidbody>().useGravity = true;
            if (!m_HasAlreadyFallen)
            {
                Vector3 forceDirection =
                    new Vector3(Random.Range(-1f, 1f), Random.Range(0, 1f), Random.Range(0f, 1f)).normalized;
                float forceMagnitude = 0.5f;
                m_Cubes[i].GetComponent<Rigidbody>().AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
            }

            if(i == m_Cubes.Count - 1) m_HasAlreadyFallen = true;
        }
    }
}

