using System.Collections;
using UnityEngine;

public class PropsInstructions : MonoBehaviour
{
   public bool canBeDestroy;
   [Tooltip("Ce props est un item qui peut être ramasser")] public bool isLoot;

   public string lootName;

   public IEnumerator DestroyThis()
   {
      while (true)
      {
         if (canBeDestroy)
         {
            if (!isLoot)
            {
               GameManager.instance.DropItem(lootName, transform);
            }

            yield return new WaitForSeconds(0.001f);
            
            Destroy(gameObject);

            break;
         }
      }
     
   }
}
