using Unity.Mathematics;
using UnityEngine;

public class BearBehaviour : AIBrain
{
    public StateMachine stateMachine;
    public enum StateMachine{IDLE, CHASE, ATTACK, ONGROUND}

    public float attackZoneRange;
    public float stunDuration;

    public float attackRangeDeadZone;

    public bool isAttacking;

    public GameObject FeedbackWarningAttack;

    private bool canSpawn;
    
    private void Start()
    {
        InitializationData();
        
        isInvincible = true;
        nav.speed = moveSpeed;
        nav.stoppingDistance = attackRange + 0.02f;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            //StartCoroutine(Death());
        }
        else
        {
            CheckState();
            Detection();
        }
    }
    
    void CheckState()
    {
        //invincible while he is not onground
        isInvincible = !isFalling;

        if (!isAggro && !isAttacking && !isFalling && canMove)
        {
            animator.Play("B_Idle");
        }

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

        if (isFalling)
        {
            isAttacking = false;
            FallOnTheGround();
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
    }

    void FallOnTheGround()
    {
        animator.Play("B_Fall");
        timeOnGround += Time.deltaTime;
        canMove = false;

        if (timeOnGround >= fallTime)
        {
            isFalling = false;
            timeOnGround = 0;

            animator.Play("B_StandUp");
        }
    }

    private void SpecialBearAttack()
    {
        AttackPlayer();
        isAttacking = true;
        
        if (!canSpawn) return;
        Instantiate(FeedbackWarningAttack, transform.position, quaternion.identity);
        canSpawn = false;
    }
}
