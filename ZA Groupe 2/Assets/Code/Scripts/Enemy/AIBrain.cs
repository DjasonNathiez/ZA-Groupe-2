using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class AIBrain : MonoBehaviour
{
    [HideInInspector] public SpawnArea spawnPoint;

    [HideInInspector] public Rigidbody rb;
    public GameObject player;
    public NavMeshAgent nav;
    [HideInInspector] public Animator animator;
    [HideInInspector] public float distanceToPlayer;

    [HideInInspector] public bool isEnable = true;

    [Header("State")]
    public int currentHealth;
    public int maxHealth;
    [HideInInspector] public bool isInvincible;
    
    public float fallTime;
    [HideInInspector] public float timeOnGround;
   
    public bool canFall;
    public bool canMove = true;
    public bool canAttack = true;
    public bool canBeKnocked;
    
    [HideInInspector] public bool isFalling;
    public bool isAttacking;
    public bool isAggro;
    [HideInInspector] public bool isKnocked;
    public bool isMoving;
    [HideInInspector] public bool isDead;
    
    [Header("Movement")]
    public float moveSpeed;

    
    [Header("Attack")]
    public int attackDamage;
    public float attackRange;
    public float attackSpeed; 
    public float attackDelay;
    [HideInInspector] public float activeAttackCd;
    [HideInInspector] public bool attackOnCd;
    public float knockbackForce;
    
    [Header("Detection")]
    public float dectectionRange;
    public float massAggroRange;
    [Range(0,180)] public float detectionAngle;
    
    [Header("Animations")]
    public string attackAnimName;
    public string idleAnimName;
    public string hurtAnimName;
    public string deathAnimName;
    public string moveAnimName;
    public string standUpAnimName;
    public string fallAnimName;
    
    [Header("VFX")]
    public ParticleSystem hurtVFX;
    public ParticleSystem attackVFX;
    public ParticleSystem hitZoneVFX;
    public ParticleSystem deathVFX;

    public bool playerShowBack;

    public void InitializationData()
    {
        currentHealth = maxHealth;
        canMove = true;
        Enable();
        
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        player = GameManager.instance.player;
        nav = GetComponent<NavMeshAgent>();
        
        nav.speed = moveSpeed;
        nav.stoppingDistance = attackRange + 0.02f;
        
        animator.Play(idleAnimName);
    }

    public void MoveToPlayer(Vector3 destination)
    {
        isMoving = true;
        animator.Play(moveAnimName);
        nav.SetDestination(destination);
    }
    
    public void Detection()
    {
        //Detect the player to aggro
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        Collider[] hit = Physics.OverlapSphere(transform.position, dectectionRange);
        
        foreach (Collider col in hit)
        {
            if (col.GetComponent<PlayerManager>())
            {
                isAggro = true;
            }
        }

        //Mass Aggro AI Comportement
        if (isAggro)
        {
            Collider[] hitMass = Physics.OverlapSphere(transform.position, massAggroRange);

            foreach (Collider c in hitMass)
            {
                AIBrain neighborAI = c.GetComponent<AIBrain>();
                
                if (neighborAI != null && !neighborAI.isAggro)
                {
                    neighborAI.isAggro = true;
                }
            }
        }

        
        float directionAngle = Vector3.Angle(player.transform.forward, transform.forward);

        playerShowBack = directionAngle < detectionAngle;

    }

    public void AttackPlayer()
    {
        isAttacking = true;
        isMoving = false;
        
        animator.Play(attackAnimName);

        canAttack = false;
    }
    
    public IEnumerator WaitForCooldown()
    {
        yield return new WaitForSeconds(attackDelay);
        Debug.Log("attack cooldown over");
        canAttack = true;
    }

    public void AttackVFX()
    {
        if (attackVFX != null)
        {
            attackVFX.Play();
        }
    }

    public void AttackExit()
    {
        isAttacking = false;
        canMove = true;
    }
    
    public void FallOnTheGround()
    {
        //Set State
        isFalling = true;
        isInvincible = false;
        isAttacking = false;
        canAttack = false;
        canMove = false;

        //Load graphics
        animator.Play(fallAnimName);

        if (hitZoneVFX != null)
        {
            hitZoneVFX.gameObject.SetActive(true);
        }

        StartCoroutine(WaitForStand());
    }
    
    #region Routine
    
   
    
    public IEnumerator WaitForStand()
    {
        yield return new WaitForSeconds(fallTime);
        isFalling = false;
        
        animator.Play(standUpAnimName);
        
        if (hitZoneVFX != null)
        {
            hitZoneVFX.gameObject.SetActive(false);  
        }
    }

    #endregion
    
    public void DoDamage()
    {
        if (distanceToPlayer < attackRange + 0.02)
        {
            player.GetComponent<PlayerManager>().GetHurt(attackDamage);

            Vector3 dir = player.transform.position - transform.position;
            dir = new Vector3(dir.x, 0, dir.z).normalized * knockbackForce;
            player.GetComponent<PlayerManager>().rb.AddForce(dir, ForceMode.Impulse);
        }
    }


    public void GetHurt(int damage)
    {
        if (!isDead)
        {
            if (isInvincible) return;
            
            currentHealth -= damage;

            if (hurtVFX != null)
            {
                hurtVFX.Play();
            }

            if (!isFalling)
            {
                animator.Play(hurtAnimName);
            }
            
            if (currentHealth <= 0)
            {
                Death();
            }
        }
    }

    public void SetSpawnPoint(SpawnArea spawnArea)
    {
        spawnPoint = spawnArea;
    }

    public void Death()
    {
        isDead = true;
        
        Disable();
        GetComponent<CapsuleCollider>().isTrigger = true;
        
        animator.Play(deathAnimName);

        if (deathVFX != null)
        {
            deathVFX.Play();
        }
        
        GameManager.instance.enemyList.Remove(this);
        
        StartCoroutine(WaitForDestroy());
    }

    IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);
    }

    public void LoadVFX(ParticleSystem effect)
    {
        effect.Play();
    }
    
    public void Enable()
    {
        isEnable = true;
    }
    
    public void Disable()
    {
        isEnable = false;

        nav.ResetPath();
        nav.SetDestination(transform.position);
        rb.velocity = Vector3.zero;
    }


    #region INSPECTOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dectectionRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    #endregion
    
}

