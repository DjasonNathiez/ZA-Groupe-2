using UnityEngine;
using UnityEngine.AI;

public class LionBehaviour : AIBrain
{
    [Header("State Informations")]
    public StateMachine stateMachine;
    public enum StateMachine{IDLE, CHASE, ATTACK,STANDUP, FALL, DEATH, HURT}
    
    private void Start()
    {
        InitializationData();
        activeAttackCd = attackDelay;
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
                animator.Play("L_Idle");
                break;
            
            case StateMachine.CHASE:
                animator.Play("L_Chase");
                ChasePlayer();
                break;
            
            case StateMachine.ATTACK:
                AttackPlayer();
                break;
            case StateMachine.STANDUP:
                animator.Play("L_StandUp");     
                break;
            case StateMachine.FALL:
                animator.Play("L_Fall");     
                break;
            case StateMachine.DEATH:
                animator.Play("L_Death");     
                break;
            case StateMachine.HURT:
                animator.Play("L_Hurt");     
                break;
        }

    }
  
    
}
