using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
   public bool isAttacking;
   private void OnTriggerEnter(Collider other)
   {
      AIBrain iaBrain = other.GetComponent<AIBrain>();
      
      if (iaBrain)
      {
         if (iaBrain.canBeAttacked)
         {
            iaBrain.GetHurt(PlayerManager.instance.attackDamage);
         }
      }
      
   }
}
