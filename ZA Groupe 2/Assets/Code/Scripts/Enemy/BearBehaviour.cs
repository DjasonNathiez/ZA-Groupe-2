using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class BearBehaviour : AIBrain
{
    [Header("Bear Self Data")]
    public float attackZoneRange;
    public float stunDuration;
    public float attackRangeDeadZone;

    public GameObject feedbackWarningAttack;
    private bool m_canSpawn;

    private void Start()
    {
        InitializationData();
    }

    private void Update()
    {
        if (!isDead)
        {
            CheckState();
            Detection();
        }

    }
    
    void CheckState()
    {
        //invincible while he is not onground
        isInvincible = !isFalling;

        if (!isFalling)
        {

            if (distanceToPlayer > attackRange + attackRangeDeadZone)
            {
                if (isAggro && !isAttacking && canMove)
                {
                    animator.Play("B_Chase");
                    MoveToPlayer(player.transform.position);
                }
            }
            else
            {
                if (isAggro)
                {
                    SpecialBearAttack();
                }
            }
           

        }
    }

    void BearZoneAttack()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, attackZoneRange);

        foreach (Collider col in hit)
        {
            if (col.GetComponent<PlayerManager>())
            {
                Debug.Log("Player hit !");
                PlayerManager.instance.GetHurt(attackDamage);
                StartCoroutine(PlayerManager.instance.StartStun(stunDuration));
            }
        }
    }

    void AttackReset()
    {
        isAttacking = false;
    }

    public void ResetMove()
    {
        canMove = true;
        canAttack = true;
    }

   

    private void SpecialBearAttack()
    {
        AttackPlayer();
        isAttacking = true;
        
        if (!m_canSpawn) return;
        Instantiate(feedbackWarningAttack, transform.position, quaternion.identity);
        m_canSpawn = false;
    }
}
