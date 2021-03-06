using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public BoxCollider m_collider;
    public bool isAttacking;
    public bool canHurt;

    public GameObject popcornVFX;

    public float knockbackForce;

    private void Awake()
    {
        m_collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    { 
      AIBrain iaBrain = other.GetComponentInParent<AIBrain>();
      BossBehaviour bossBehaviour = other.GetComponent<BossBehaviour>();
      KnockableObject knockableObject = other.GetComponent<KnockableObject>();
      PropsInstructions props = other.GetComponent<PropsInstructions>();

      if (props && isAttacking)
      {
          StartCoroutine(props.DestroyThis());
          GetComponentInParent<PlayerManager>().PlaySFX("P_AttackHit");
      }

      if (iaBrain && other.CompareTag("HitBox"))
      {
          
          if (canHurt)
          {
              GetComponentInParent<PlayerManager>().PlaySFX("P_AttackHit");
              iaBrain.GetHurt(PlayerManager.instance.attackDamage);
              
              if (iaBrain.canBeKnocked)
              {
                  iaBrain.Disable();
                  iaBrain.GetComponent<AIBrain>().rb.isKinematic = false;
                  
                  Vector3 dir = iaBrain.gameObject.transform.position - transform.position;
                  dir.y = 0.1f;
                  iaBrain.GetComponent<AIBrain>().rb.AddForce(dir * knockbackForce, ForceMode.Impulse);

                  iaBrain.Enable();
              }
              
              
              iaBrain.GetComponent<AIBrain>().rb.isKinematic = true;
              canHurt = false;
          }

      }
      
      if (bossBehaviour)
      {
          if (canHurt)
          {
              GetComponentInParent<PlayerManager>().PlaySFX("P_AttackHit");
              bossBehaviour.GetHurt(PlayerManager.instance.attackDamage);
              canHurt = false;
          }

      }
      
      if (knockableObject)
      {
          GetComponentInParent<PlayerManager>().PlaySFX("P_AttackHit");
          Vector3 dir = other.transform.position - PlayerManager.instance.transform.position;
          dir = new Vector3(dir.x, knockableObject.yforce, dir.z).normalized * knockableObject.force;
          knockableObject.rb.AddForce(dir,ForceMode.Impulse);
          knockableObject.rb.isKinematic = false;

          if (knockableObject.isHit)
          {
              Instantiate(popcornVFX, transform.position, quaternion.identity);
              Destroy(knockableObject.popcornInterior);
              knockableObject.isHit = false; 
          }
      }

    }
}
