using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LionBehaviour : AIBrain
{
    [Header("State Informations")]
    public StateMachine stateMachine;
    public enum StateMachine{IDLE, CHASE, ATTACK}

    public float timerToResetCounterState;
    
    [Header("VFX)")]
    public ParticleSystem attackVFX;

    private bool isAttacking;
    
    private void Start()
    {
        //isInvincible = true;
        canAttack = true;
        
        InitializationData();
        activeAttackCd = attackDelay;
        nav.speed = moveSpeed;
        nav.stoppingDistance = attackRange + 0.02f;
        
        animator.Play("L_Idle");
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            Death();
            animator.Play("L_Death");
        }
        else
        {
            if (!isDead && !isFalling)
            {
                CheckState();
                Detection();
            }
        }
        
        if (isFalling)
        {
            isAttacking = false;
            FallOnTheGround();
        }
    }

    private void CheckState()
    {
        if (!isDead)
        {
            if (!isFalling)
            {
                if (distanceToPlayer > attackRange)
                {
                    if (isAggro && !isAttacking && canMove)
                    {
                        animator.Play("L_Chase");
                        ChasePlayer();
                    }
                }
                else
                {
                    if (isAggro && canAttack)
                    {
                        AttackPlayer();
                    }
                }
            }
        }


        if (isFalling)
        {
            isAttacking = false;
            FallOnTheGround();
        }
    }

    public void ResetMove()
    {
        canMove = true;
        canAttack = true;
    }

    void FallOnTheGround()
    {
        animator.Play("L_Fall");
        timeOnGround += Time.deltaTime;
        canMove = false;
        canAttack = false;

        if (timeOnGround >= fallTime)
        {
            isFalling = false;
            timeOnGround = 0;

            animator.Play("L_StandUp");
        }
    }
    public IEnumerator ResetInvincibility()
    {
        yield return new WaitForSeconds(timerToResetCounterState);
        isInvincible = true;
        animator.Play("L_StandUp");
        isFalling = false;
    }

    public void EnableMove()
    {
        canMove = true;
    }
    
    public void LoadAttackVFX()
    {
        attackVFX.Play();
    }
    public void PlayAnim(string animName)
    {
        animator.Play(animName);
    }
    
    public void StopCounterState()
    {
        isInvincible = false;
        canMove = false;
        isFalling = true;
        
        //Play Anim Break;
        //Play VFX Break;
    }
  
    
}
