using UnityEngine;

public class RopeBox : MonoBehaviour
{
   public float additionValue;
   private void OnTriggerEnter(Collider other)
   {
      PlayerManager playerManager = other.GetComponent<PlayerManager>();

      if (playerManager)
      {
         playerManager.rope.maximumLenght += additionValue;
         Debug.Log("add rope lenght");
         
         Destroy(gameObject);
      }
   }
}
