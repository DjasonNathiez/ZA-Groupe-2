using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingWeapon : MonoBehaviour
{
    [SerializeField] private PlayerManager m_PlayerManager;
    public Transform grip;
    private void OnTriggerEnter(Collider other)
    {
        if (m_PlayerManager.state == "Throw")
        {
            if (other.CompareTag("GrippableObject"))
            {
                m_PlayerManager.state = "Rope";
                if (other.ClosestPoint(transform.position) == transform.position)
                {
                    grip.position = other.ClosestPoint(m_PlayerManager.transform.position) - transform.forward * 0.3f;
                }
                else
                {
                    grip.position = other.ClosestPoint(transform.position) - transform.forward * 0.3f;   
                }
                grip.parent = other.transform;
            }
            else if (other.CompareTag("UngrippableObject"))
            {
                Debug.Log("HEEEEY YA");
                m_PlayerManager.state = "Rope";
                m_PlayerManager.Rewind();
            }
            else if (other.CompareTag("TractableObject"))
            {
                m_PlayerManager.state = "Rope";
                m_PlayerManager.m_rope.pinnedToObject = true;
                m_PlayerManager.m_rope.pinnedRb = other.attachedRigidbody;
                m_PlayerManager.m_rope.pinnedObjectDistance = m_PlayerManager.m_rope.lenght;
                grip.position = transform.position;
                grip.parent = other.transform;
            }
        }
    }
    
    private void Update()
    {
        if (m_PlayerManager.state == "Rope" && !m_PlayerManager.m_rope.rewinding)
        {
            transform.position = grip.position;
        }
        
        if (m_PlayerManager.state == "Throw" && m_PlayerManager.m_rope.lenght >= m_PlayerManager.m_rope.maximumLenght)
        {
            m_PlayerManager.m_rope.rewinding = true;
        }
    }
}
