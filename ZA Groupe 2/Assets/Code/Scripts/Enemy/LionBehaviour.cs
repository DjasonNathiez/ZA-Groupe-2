using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LionBehaviour : AIBrain
{
    [Header("Lion Self Data")]
    public float timerToResetCounterState;

    public ParticleSystem fallVFX;
    public ParticleSystem standUpVFX;
    
    private void Start()
    {
        isInvincible = true;
        canAttack = true;
        
        InitializationData();
        activeAttackCd = attackDelay;
        nav.speed = moveSpeed;
        nav.stoppingDistance = attackRange + 0.02f;
        
        animator.Play("L_Idle");
    }

    private void Update()
    {
       if (!isDead)
       {
          CheckState();
          Detection();
       }
       else
       {
           StopCoroutine(ResetInvincibility());
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
                    if (isAggro)
                    {
                        AttackPlayer();
                    }
                }
            }
        }


        if (isFalling)
        {
            isInvincible = false;
            isAttacking = false;
            FallOnTheGround(); 
        }
    }

    public void ResetMove()
    {
        if (!isDead)
        {
            canMove = true;
            canAttack = true;
            animator.Play("L_Idle");
        }
    }

    void FallOnTheGround()
    {
        if (!isDead)
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
                standUpVFX.Play();
            }
        }
    }
    public IEnumerator ResetInvincibility()
    {
        if (!isDead)
        {
            yield return new WaitForSeconds(timerToResetCounterState);
            isInvincible = true;
            animator.Play("L_StandUp");
            standUpVFX.Play();
            isFalling = false;
        }
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
        if (!isDead)
        {
            animator.Play(animName);
        }
    }
    
    public void StopCounterState()
    {
        if (!isDead)
        {
            isInvincible = false;
            canMove = false;
            isFalling = true;

            fallVFX.Play();
            //Play Anim Break;
            //Play VFX Break;
        }
    }
  
    
}
