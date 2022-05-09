using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LionBehaviour : AIBrain
{
    [Header("State Informations")]
    public StateMachine stateMachine;
    public enum StateMachine{IDLE, CHASE, ATTACK}

    public float timerToResetCounterState;
    
    private void Start()
    {
        isInvincible = true;
        
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

        if (!isInvincible)
        {
            StartCoroutine(ResetInvincibility());

            IEnumerator ResetInvincibility()
            {
                yield return new WaitForSeconds(timerToResetCounterState);
                isInvincible = true;
                animator.Play("L_StandUp");
                canMove = true;
            }
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
                StartCoroutine(LoadAttack());
                
                IEnumerator LoadAttack()
                {
                    yield return new WaitForSeconds(0.5f);
                    AttackPlayer();
                }
                break;
        }

    }

    public void StopCounterState()
    {
        isInvincible = false;
        canMove = false;
        animator.Play("L_Fall");
        //Play Anim Break;
        //Play VFX Break;
    }
  
    
}
