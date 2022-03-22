using UnityEngine;

public class BearBehaviour : AIBrain
{
    public StateMachine stateMachine;
    public enum StateMachine{IDLE, CHASE, ATTACK, ONGROUND}
    
    
    private void Start()
    {
        InitializationData();
        
        isInvincible = true;
        m_nav.speed = moveSpeed;
        m_nav.stoppingDistance = attackRange + 0.02f;
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
    }
    
    void CheckState()
    {
        if (isAggro && !isFalling)
        {
            stateMachine = distanceToPlayer > attackRange +0.02 ? StateMachine.CHASE : StateMachine.ATTACK;
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
                ChasePlayer();
                break;
            
            case StateMachine.ATTACK:
                AttackPlayer();
                break;
            
            case StateMachine.ONGROUND:
                FallOnTheGround();
                break;
        }
    }

    void FallOnTheGround()
    {
        timeOnGround += Time.deltaTime;

        if (timeOnGround >= fallTime)
        {
            isFalling = false;
            timeOnGround = 0;
            
            DebugSetColor(backupColor);
        }
    }
}
