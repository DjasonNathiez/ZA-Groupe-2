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
        
        isInvincible = true;
        nav.speed = moveSpeed;
        nav.stoppingDistance = attackRange + 0.02f;
        
        animator.Play("B_Idle");
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
                    ChasePlayer();
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

    public void FallOnTheGround()
    {
        //Set State
        isFalling = true;
        isInvincible = false;
        isAttacking = false;
        Disable();
        
        //Load graphics
        animator.Play("B_Fall");
        hitZoneVFX.gameObject.SetActive(true);
        
        
        timeOnGround += Time.deltaTime;
        
        if (timeOnGround >= fallTime)
        {
            isFalling = false;
            timeOnGround = 0;

            animator.Play("B_StandUp");
            hitZoneVFX.gameObject.SetActive(false);
            
            Enable();
        }
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
