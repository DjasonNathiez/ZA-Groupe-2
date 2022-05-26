using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LionBehaviour : AIBrain
{
    [Header("Lion Self Data")] 
    public bool stayAtRange;
    public float awayRange;
    public Vector3 awayPoint;
    public Vector3 target;
    public float rushSpeed;
    
    public float timerToResetCounterState;

    public ParticleSystem fallVFX;
    public ParticleSystem standUpVFX;
    
    private void Awake()
    {
        InitializationData();
    }

    private void Update()
    {
        //AnimatorSetBool
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isDead", isDead);
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isHurt", isHurt);
        animator.SetBool("isMoving", isMoving);

        counterState = isInvincible = !isFalling;
        
        Detection();
        
        if (isEnable && !isDead)
        {
            stayAtRange = !playerShowBack;
            
            if (isAggro)
            {

                if (canMove && !isAttacking)
                {
                    if (stayAtRange)
                    {
                        if (distanceToPlayer < awayRange)
                        {
                            awayPoint = new Vector3(player.transform.position.x - awayRange, player.transform.position.y, player.transform.position.z - awayRange);
                            
                            NavMeshHit navHit;
                            if (NavMesh.SamplePosition(target, out navHit, awayRange, NavMesh.AllAreas))
                            {
                                awayPoint = -awayPoint;
                            }
                        }
                       
                        if(distanceToPlayer >= awayRange)
                        {
                            awayPoint = player.transform.position;
                        }

                        nav.speed = moveSpeed;
                        target = awayPoint;
                    }
                    else
                    {
                        if (distanceToPlayer > attackRange)
                        {
                            nav.speed = rushSpeed;
                            target = player.transform.position;
                        }
                    }
                    
                    MoveToPlayer(target);
                }

                
                
                if (distanceToPlayer <= attackRange)
                {
                    if (canAttack)
                    {
                        AttackPlayer();
                    }
                }
            }

            if (isFalling)
            {
                canAttack = false;
                canMove = false;
                canFall = false;
            }
            else
            {
                if (isAggro)
                {
                    transform.LookAt(player.transform);
                }
            }
        }
    }

    public void StopCounterState()
    {
        if (!isDead)
        {
            isInvincible = false;
            canMove = false;
            isFalling = true;
            
            FallOnTheGround();
            fallVFX.Play();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, awayRange);
    }
}
