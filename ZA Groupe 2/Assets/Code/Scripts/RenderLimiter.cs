using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RenderLimiter : MonoBehaviour
{
   public List<GameObject> objects;
   public List<RenderLimiter> allLimiters;

   private void Awake()
   {
      allLimiters.AddRange(FindObjectsOfType<RenderLimiter>());
      
      if (allLimiters.Contains(this))
      {
         allLimiters.Remove(this);
      }
   }

   private void OnTriggerEnter(Collider other)
   {
      if (!other.CompareTag("Ground") && !other.CompareTag("Player"))
      {
         foreach (RenderLimiter r in allLimiters)
         {
            if (r.objects.Contains(other.gameObject))
            {
               r.objects.Remove(other.gameObject);
            }
         }
      
         if (!objects.Contains(other.gameObject))
         {
            objects.Add(other.gameObject);
         }
      }

      if (other.CompareTag("Player"))
      {
         UpdateObjects();
      }
   }

   public void UpdateObjects()
   {
         foreach (GameObject obj in this.objects)
         {
            if (!obj.activeSelf)
            {
               MeshRenderer objRenderer = obj.GetComponent<MeshRenderer>();
               
               objRenderer.shadowCastingMode = ShadowCastingMode.On;
               objRenderer.lightProbeUsage = LightProbeUsage.BlendProbes;
               objRenderer.allowOcclusionWhenDynamic = true;
            }
         }
      
         foreach (RenderLimiter r in allLimiters)
         {
            //Set an priority order to things to disable or modify render
            
            foreach (GameObject obj in r.objects)
            {
               MeshRenderer objRenderer = obj.GetComponent<MeshRenderer>();
               objRenderer.shadowCastingMode = ShadowCastingMode.Off;
               objRenderer.lightProbeUsage = LightProbeUsage.Off;
               objRenderer.allowOcclusionWhenDynamic = false;
            }
         }
   }

   public void DisableObject()
   {
      foreach (RenderLimiter r in allLimiters)
      {
         foreach (GameObject obj in r.objects)
         {
            obj.SetActive(false);
         }
         Debug.Log("Disable Object in " + r.gameObject.name + " limiter");
      }
   }
}
