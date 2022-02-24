using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
   public bool isAttacking;
   private void OnTriggerEnter(Collider other)
   {
      if (other.GetComponent<AIBrain>().canBeAttacked && isAttacking)
      {
         other.GetComponent<AIBrain>().GetHurt(PlayerManager.instance.attackDamage);
      }
      
   }
}
