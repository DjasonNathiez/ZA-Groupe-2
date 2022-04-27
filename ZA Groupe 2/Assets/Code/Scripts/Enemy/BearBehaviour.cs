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
            StartCoroutine(Death());
        }
        else
        {
            CheckState();
            Detection();
        }
        if (isAttacking == true)
        {
            Instantiate(FeedbackWarningAttack, transform.position, Quaternion.identity);

        }
    }
    
    void CheckState()
    {
        if (isAggro && !isFalling && !isAttacking)
        {
            stateMachine = distanceToPlayer > attackRange + attackRangeDeadZone ? StateMachine.CHASE : StateMachine.ATTACK;
        }

        if (isFalling)
        {
            stateMachine = StateMachine.ONGROUND;
        }

        //invincible while he is not onground
        isInvincible = stateMachine != StateMachine.ONGROUND;
        
        switch (stateMachine)
        {
            case StateMachine.CHASE:
                animator.Play("B_Chase");
                ChasePlayer();
                break;
            
            case StateMachine.ATTACK:
                AttackPlayer();
                isAttacking = true;
                break;
            
            case StateMachine.ONGROUND:
                FallOnTheGround();
                break;
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

    void FallOnTheGround()
    {
        animator.Play("B_Fall");
        timeOnGround += Time.deltaTime;

        if (timeOnGround >= fallTime)
        {
            isFalling = false;
            timeOnGround = 0;

            animator.Play("B_StandUp");
            DebugSetColor(backupColor);
        }
    }
}
