using Unity.Mathematics;
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
      AIBrain iaBrain = other.GetComponent<AIBrain>();
      KnockableObject knockableObject = other.GetComponent<KnockableObject>();
      PropsInstructions props = other.GetComponent<PropsInstructions>();

      if (props)
      {
          StartCoroutine(props.DestroyThis());
      }

      if (iaBrain)
      {
          if (canHurt)
          {
              iaBrain.GetHurt(PlayerManager.instance.attackDamage);
              
              if (iaBrain.canKnockback)
              {
                  Vector3 dir = iaBrain.gameObject.transform.position - transform.position;
                  dir.y = 0.1f;
                  iaBrain.GetComponent<AIBrain>().rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
              
                  iaBrain.GetComponent<AIBrain>().rb.isKinematic = false;
              }
              
              canHurt = false;
          }

      }
      
      if (knockableObject)
      {
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
