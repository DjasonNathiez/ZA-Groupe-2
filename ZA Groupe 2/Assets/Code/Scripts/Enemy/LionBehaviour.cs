using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class LionBehaviour : AIBrain
{
    [Header("Lion Self Data")] public bool stayAtRange;
    public float awayRange;
    public Vector3 awayPoint;
    public Vector3 target;
    public float rushSpeed;
    [Range(0, 180)] public float detectionAngle;
    
    public ParticleSystem fallVFX;
    public ParticleSystem standUpVFX;
    public ParticleSystem counterVFX;

    [HideInInspector] public bool playerShowBack;
    
    private void Start()
    {
        InitializationData();
    }

    public override void Update()
    {
        base.Update();

        if (isFalling) return;

        if (canAttack) Detection();

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
                
                if (!isFalling && nav.enabled)
                {
                    transform.LookAt(player.transform);
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                }
            }
        }
    }

    public override void GetHurt(int damage)
    {
        if (!isFalling || isInvincible)
        {
            if(!counterVFX.isPlaying) counterVFX.Play();
            return;
        }
        base.GetHurt(damage);
    }

    public override void SetAnimator()
    {
       base.SetAnimator();
       animator.SetBool("isFalling", isFalling);

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
            Debug.Log("dead");
            modelAggroMat.SetFloat("_NoiseStrenght", animationDeath.Evaluate(Time.time - hurtTime));
            modelNonAggroMat.SetFloat("_NoiseStrenght", animationDeath.Evaluate(Time.time - hurtTime));
        }
    }

    public override void Detection()
    {
        base.Detection();

        if (distanceToPlayer <= dectectionRange && !isAggro)
        {
            if (!isDead)
            {
                isAggro = true;
                PlaySFX("L_Aggro");
                if(!aggroVFX.isPlaying) aggroVFX.Play();
            }
        }
        
        float directionAngle = Vector3.Angle(player.transform.forward, transform.forward);

        playerShowBack = directionAngle < detectionAngle;
    }

    public override void MoveToPlayer(Vector3 destination)
    {
        base.MoveToPlayer(destination);
        
    }

    public void StopCounterState()
    {
        if (!isDead)
        {
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
                if (s.clip == null)
                {
                    Debug.LogWarning("Audio Clip has not been found !");
                    return;
                }

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