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
      KnockableObject knockableObject = other.GetComponent<KnockableObject>();

      if (iaBrain)
      {
          iaBrain.GetHurt(PlayerManager.instance.attackDamage);
      }
      
      if (knockableObject)
      {
          Vector3 dir = other.transform.position - PlayerManager.instance.transform.position;
          dir = new Vector3(dir.x, knockableObject.yforce, dir.z).normalized * knockableObject.force;
          knockableObject.rb.AddForce(dir,ForceMode.Impulse);
          knockableObject.rb.isKinematic = false;
      }

   }
}
