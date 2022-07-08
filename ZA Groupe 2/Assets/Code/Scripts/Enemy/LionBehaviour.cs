using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LionBehaviour : AIBrain
{
    [Header("Lion Self Data")] public bool stayAtRange;
    public float awayRange;
    public Vector3 awayPoint;
    public Vector3 target;
    public float rushSpeed;

    public float timerToResetCounterState;

    public ParticleSystem fallVFX;
    public ParticleSystem standUpVFX;

    private void Start()
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

        if (hurtAnim)
        {
            modelAggroMat.SetFloat("_UseOnAlbedo", animationHurt.Evaluate(Time.time - hurtTime));
            modelAggroMat.SetFloat("_UseOnEmission", animationHurt.Evaluate(Time.time - hurtTime));
            modelNonAggroMat.SetFloat("_UseOnAlbedo", animationHurt.Evaluate(Time.time - hurtTime));
            modelNonAggroMat.SetFloat("_UseOnEmission", animationHurt.Evaluate(Time.time - hurtTime));
            if (Time.time - hurtTime > animationHurt.keys[animationHurt.keys.Length - 1].time) hurtAnim = false;
        }

        if (isDead)
        {
            modelAggroMat.SetFloat("_NoiseStrenght", animationDeath.Evaluate(Time.time - hurtTime));
            modelNonAggroMat.SetFloat("_NoiseStrenght", animationDeath.Evaluate(Time.time - hurtTime));
        }

        counterState = isInvincible = !isFalling;

        Detection();
        SetColor();

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
                            awayPoint = new Vector3(player.transform.position.x - awayRange,
                                player.transform.position.y, player.transform.position.z - awayRange);

                            NavMeshHit navHit;
                            if (NavMesh.SamplePosition(target, out navHit, awayRange, NavMesh.AllAreas))
                            {
                                awayPoint = -awayPoint;
                            }
                        }

                        if (distanceToPlayer >= awayRange)
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

        enemyStatusPointer.SetActive(isAggro);
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

    public void PlaySFX(string soundName)
    {
        foreach (AudioManager.Sounds s in AudioManager.instance.lionSounds)
        {
            if (s.soundName == soundName)
            {
                if (s.loop)
                {
                    SoundManager.PlayFx(s.clip, loop: true, volume: s.volume);
                }
                else
                {
                    SoundManager.PlayOnce(s.clip, volume: s.volume, mixer: SoundManager.FxAudioMixer);
                }
            }
        }
    }

    public void StopSFX(string soundName)
    {
        foreach (AudioManager.Sounds s in AudioManager.instance.lionSounds)
        {
            if (s.soundName == soundName)
            {
                SoundManager.StopFx(s.clip);
            }
        }
    }
}