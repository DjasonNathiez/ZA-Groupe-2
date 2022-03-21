using UnityEngine;
using UnityEngine.AI;

public class LionBehaviour : AIBrain
{
    [Header("State Informations")]
    public StateMachine stateMachine;
    public enum StateMachine{IDLE, CHASE, ATTACK}
    
    private void Start()
    {
        InitializationData();
        m_activeAttackCD = m_attackDelay;
        m_nav.speed = moveSpeed;
        m_nav.stoppingDistance = attackRange + 0.02f;
    }

    private void Update()
    {
        CheckState();
        Detection();
    }

    private void CheckState()
    {
        //SET STATES BY CONDITIONS
        if (isAggro)
        {
            stateMachine = distanceToPlayer > attackRange +0.02 ? StateMachine.CHASE : StateMachine.ATTACK;
        }
        
        //APPLY STATES ACTION
        switch (stateMachine)
        {
            case StateMachine.IDLE:
                animator.Play("ennemy_lion_idle");
                break;
            
            case StateMachine.CHASE:
                animator.Play("ennemy_lion_idle");
                ChasePlayer();
                break;
            
            case StateMachine.ATTACK:
                AttackPlayer();
                break;
        }
        
        if (currentHealth <= 0)
        {
            Death();
        }
        
    }
  
    
}
