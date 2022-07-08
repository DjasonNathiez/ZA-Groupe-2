using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class BearBehaviour : AIBrain
{
    [Header("Bear Self Data")] public float attackZoneRange;
    public float stunDuration;
    public float attackRangeDeadZone;

    public GameObject feedbackWarningAttack;
    private bool m_canSpawn;

    public CapsuleCollider bodyCol;

    private void Start()
    {
        InitializationData();
    }

    private void Update()
    {
        //bodyCol.isTrigger = !isFalling;

        //Animator Set Bool
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isHurt", isHurt);
        animator.SetBool("isDead", isDead);
        if (isDead)
        {
            modelAggroMat.SetFloat("_NoiseStrenght", animationDeath.Evaluate(Time.time - hurtTime));
            modelNonAggroMat.SetFloat("_NoiseStrenght", animationDeath.Evaluate(Time.time - hurtTime));
        }

        if (hurtAnim)
        {
            modelAggroMat.SetFloat("_UseOnAlbedo", animationHurt.Evaluate(Time.time - hurtTime));
            modelAggroMat.SetFloat("_UseOnEmission", animationHurt.Evaluate(Time.time - hurtTime));
            modelNonAggroMat.SetFloat("_UseOnAlbedo", animationHurt.Evaluate(Time.time - hurtTime));
            modelNonAggroMat.SetFloat("_UseOnEmission", animationHurt.Evaluate(Time.time - hurtTime));
            if (Time.time - hurtTime > animationHurt.keys[animationHurt.keys.Length - 1].time) hurtAnim = false;
        }

        if (!isDead)
        {
            CheckState();
            Detection();
        }

        SetColor();
        enemyStatusPointer.SetActive(isAggro);
    }

    void CheckState()
    {
        //invincible while he is not onground
        isInvincible = !isFalling;
        GetComponent<Collider>().isTrigger = isFalling;

        if (!isFalling)
        {
            if (distanceToPlayer > attackRange + attackRangeDeadZone)
            {
                if (isAggro && !isAttacking && canMove)
                {
                    MoveToPlayer(player.transform.position);
                }
            }
            else
            {
                if (isAggro)
                {
                    SpecialBearAttack();
                }
            }
        }
    }

    void BearZoneAttack()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, attackZoneRange);

        foreach (Collider col in hit)
        {
            if (col.GetComponent<PlayerManager>())
            {
                PlayerManager.instance.GetHurt(attackDamage);
                StartCoroutine(PlayerManager.instance.StartStun(stunDuration));
            }
        }
    }

    void AttackReset()
    {
        isAttacking = false;
    }

    public void ResetMove()
    {
        canMove = true;
        canAttack = true;
        isFalling = false;
    }


    private void SpecialBearAttack()
    {
        AttackPlayer();
        isAttacking = true;

        if (!m_canSpawn) return;
        Instantiate(feedbackWarningAttack, transform.position, quaternion.identity);
        m_canSpawn = false;
    }

    public void PlaySFX(string soundName)
    {
        foreach (AudioManager.Sounds s in AudioManager.instance.bearSounds)
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
        foreach (AudioManager.Sounds s in AudioManager.instance.bearSounds)
        {
            if (s.soundName == soundName)
            {
                SoundManager.StopFx(s.clip);
            }
        }
    }
}