using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossDetector : MonoBehaviour
{
    [SerializeField] private BossBehaviour m_bossBehaviour;
    private void OnTriggerEnter(Collider other)
    {
        if (m_bossBehaviour.state == 2)
        {
            if (other.gameObject.CompareTag("UngrippableObject"))
            {
                m_bossBehaviour.state = 0;
                m_bossBehaviour.rb.velocity = Vector3.zero;
                StartCoroutine(m_bossBehaviour.ReturnToIddle(1));
            }
            else if (m_bossBehaviour.pillars.Contains(other.gameObject))
            {
                m_bossBehaviour.state = 0;
                m_bossBehaviour.pillars.Remove(other.gameObject);
                Destroy(other.gameObject);
                m_bossBehaviour.rb.velocity = Vector3.zero;
                StartCoroutine(m_bossBehaviour.ReturnToIddle(1));
            }
            else if (other.gameObject.CompareTag("Player"))
            {
                m_bossBehaviour.state = 0;
                m_bossBehaviour.rb.velocity = Vector3.zero;
                
                // Faire perdre 1 point de vie au joueur ------------------------
                
                StartCoroutine(m_bossBehaviour.ReturnToIddle(1));
            }
        }
    }
}
