using Unity.Mathematics;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private new BoxCollider m_collider;
    public bool isAttacking;

    public GameObject popcornVFX;

    public float knockbackForce;

    private void Awake()
    {
        m_collider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        m_collider.enabled = isAttacking;
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
          iaBrain.GetHurt(PlayerManager.instance.attackDamage);
          
          Vector3 dir = iaBrain.gameObject.transform.position - transform.position;
          dir = new Vector3(dir.x, 0, dir.z).normalized * knockbackForce;
          iaBrain.GetComponent<AIBrain>().rb.AddForce(dir, ForceMode.Impulse);
      }
      
      if (knockableObject)
      {
          Vector3 dir = other.transform.position - PlayerManager.instance.transform.position;
          dir = new Vector3(dir.x, knockableObject.yforce, dir.z).normalized * knockableObject.force;
          knockableObject.rb.AddForce(dir,ForceMode.Impulse);
          knockableObject.rb.isKinematic = false;
      }

      if (knockableObject && knockableObject.isHit)
      {
          Instantiate(popcornVFX, transform.position, quaternion.identity);
          Destroy(knockableObject.popcornInterior);
          knockableObject.isHit = false; 
      }
    }
}
