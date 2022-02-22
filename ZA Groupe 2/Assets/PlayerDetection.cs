using System;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
   private PlayerManager m_playerManager;

   private void Start()
   {
      m_playerManager = GetComponentInParent<PlayerManager>();
   }

   private void OnTriggerEnter(Collider other)
   {
      m_playerManager.SetFrontObject(other.gameObject);
   }

   private void OnTriggerExit(Collider other)
   {
      m_playerManager.SetFrontObject(null);
   }
}
