using UnityEngine;

public class BearBehaviour : AIBrain
{
    public StateMachine stateMachine;
    public enum StateMachine{IDLE, CHASE, ATTACK, ONGROUND}
    private void Awake()
    {
        InitializationData();
    }

    private void Start()
    {
        isInvincible = true;
        m_nav.speed = moveSpeed;
        m_nav.stoppingDistance = attackRange + 0.02f;
    }

    private void Update()
    {
        CheckState();
        Detection();

        if (isFalling)
        {
            FallOnTheGround();
        }
    }

    void Chase()
    {
        m_nav.SetDestination(m_player.transform.position);
    }

    void CheckState()
    {
        if (currentHealth <= 0)
        {
            Death();
        }
        
        if (isAggro && stateMachine != StateMachine.ONGROUND)
        {
            stateMachine = distanceToPlayer > attackRange +0.02 ? StateMachine.CHASE : StateMachine.ATTACK;
        }

        //invincible while he is not onground
        isInvincible = stateMachine != StateMachine.ONGROUND;
        
        switch (stateMachine)
        {
            case StateMachine.CHASE:
                Chase();
                break;
        }
    }

    void FallOnTheGround()
    {
        stateMachine = StateMachine.ONGROUND;
    }
}
