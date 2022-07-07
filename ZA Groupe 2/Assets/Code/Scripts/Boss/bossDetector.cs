using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossDetector : MonoBehaviour
{
    [SerializeField] private BossBehaviour m_bossBehaviour;
    private void OnTriggerStay(Collider other)
    {
        if (m_bossBehaviour.state == 2)
        {
            if (other.gameObject.CompareTag("UngrippableObject"))
            {
                m_bossBehaviour.state = 0;
                m_bossBehaviour.rb.velocity = Vector3.zero;
                StartCoroutine(m_bossBehaviour.ReturnToIddle(1));
                m_bossBehaviour.animator.Play("DashEnd");
            }
            else if (m_bossBehaviour.pillars.Contains(other.gameObject))
            {
                m_bossBehaviour.state = 0;
                m_bossBehaviour.pillars.Remove(other.gameObject);
                if (PlayerManager.instance.rope.pinnedTo == other.gameObject)
                {
                    PlayerManager.instance.Rewind();
                    PlayerManager.instance.rope.pinnedTo = null;
                    PlayerManager.instance.rope.pinnedRb = null;
                    PlayerManager.instance.throwingWeapon.GetComponent<ThrowingWeapon>().grip.parent = PlayerManager.instance.throwingWeapon.transform;
                }
                Destroy(other.gameObject);
                GameObject vfxpillar = Instantiate(m_bossBehaviour.vfx[2], other.transform.position, Quaternion.identity);
                vfxpillar.transform.rotation = Quaternion.Euler(-90,0,0);
                Destroy(vfxpillar,3);
                m_bossBehaviour.rb.velocity = Vector3.zero;
                StartCoroutine(m_bossBehaviour.ReturnToIddle(1));
                m_bossBehaviour.animator.Play("DashEnd");
            }
            else if (other.gameObject.CompareTag("Player"))
            {
                m_bossBehaviour.state = 0;
                m_bossBehaviour.rb.velocity = Vector3.zero;
                
                PlayerManager.instance.GetHurt(1);

                StartCoroutine(m_bossBehaviour.ReturnToIddle(1));
            }
            else if (other.gameObject.CompareTag("Ennemi"))
            {
                m_bossBehaviour.state = 0;
                Destroy(other.gameObject);
                m_bossBehaviour.rb.velocity = Vector3.zero;
                StartCoroutine(m_bossBehaviour.ReturnToIddle(1));
                m_bossBehaviour.animator.Play("DashEnd");
            }
        }
    }
}
