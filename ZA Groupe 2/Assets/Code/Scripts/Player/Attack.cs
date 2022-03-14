using UnityEngine;

public class Attack : MonoBehaviour
{
    private BoxCollider collider;
    public bool isAttacking;

    private void Awake()
    {
        collider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        collider.enabled = isAttacking;
    }

    private void OnTriggerEnter(Collider other)
   {
      AIBrain iaBrain = other.GetComponent<AIBrain>();
      
      if (iaBrain)
      {
          iaBrain.GetHurt(PlayerManager.instance.attackDamage);
      }

   }
}
