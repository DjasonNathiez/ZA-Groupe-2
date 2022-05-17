using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class AIBrain : MonoBehaviour
{
    [HideInInspector] public SpawnArea spawnPoint;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public GameObject player;
    [HideInInspector] public NavMeshAgent nav;
    [HideInInspector] public Animator animator;
    [HideInInspector] public float distanceToPlayer;
   
    [Header("State")]
    public int currentHealth;
    public int maxHealth;
    public bool canFall;
    public float fallTime;
    
    [HideInInspector] public float timeOnGround;
    [HideInInspector] public bool isFalling;
    [HideInInspector] public bool isInvincible;
    [HideInInspector] public bool haveCounterState;
    [HideInInspector] public bool isStun;
    [HideInInspector] public bool isAggro;
    [HideInInspector] public bool canMove;
    public bool canBeKnocked;
    [HideInInspector] public bool canHurt;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isAttacking;
    
    [Header("Movement")]
    public float moveSpeed;
    
    [Header("Attack")]
    public int attackDamage;
    public float attackRange;
    public float attackSpeed; 
    public float attackDelay;
    [HideInInspector] public float activeAttackCd;
    [HideInInspector] public bool canAttack;
    [HideInInspector] public bool attackOnCd;
    public float knockbackForce;
    
    //detection
    [Header("Detection")]
    public float dectectionRange;
    
    [Header("Animations")]
    public string attackAnimName;
    public string idleAnimName;
    public string hurtAnimName;
    public string deathAnimName;

    [Header("VFX")]
    public ParticleSystem hurtVFX;
    public ParticleSystem attackVFX;
    public ParticleSystem hitZoneVFX;
    public ParticleSystem deathVFX;

    public void InitializationData()
    {
        currentHealth = maxHealth;
        canMove = true;
        
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        nav = GetComponent<NavMeshAgent>();
    }

    public void ChasePlayer()
    {
        if (canMove)
        {
            nav.SetDestination(player.transform.position);
        }
    }
    
    public void Detection()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        Collider[] hit = Physics.OverlapSphere(transform.position, dectectionRange);
        
        foreach (Collider col in hit)
        {
            if (col.GetComponent<PlayerManager>())
            {
                float distToPlayerY = transform.position.y - col.transform.position.y;

                if (distToPlayerY < 3)
                {
                    isAggro = true;
                }
            }

            if (col.GetComponent<AIBrain>())
            {
                var colEnemy = col.GetComponent<AIBrain>();

                if (colEnemy.isAggro)
                {
                    isAggro = true;
                }
            }
        }

    }
    
    public void AttackPlayer()
    {
        if (attackOnCd)
        {
            AttackCooldown();
        }
        
        if (!attackOnCd)
        {
            animator.Play(attackAnimName);
            canHurt = true;
        }
        
    }
    
    private void AttackCooldown()
    {
        switch (activeAttackCd)
            {
                case > 0:
                    canAttack = false;
                    activeAttackCd -= Time.deltaTime;
                    break;
            
                case <= 0:
                    canAttack = true;
                    activeAttackCd = attackDelay;
                    attackOnCd = false;
                    break;
            }
        
    }
    
    public void DoDamage()
    {
        if (distanceToPlayer < attackRange + 0.02 && canHurt)
        {
            Debug.Log("Player take " + attackDamage + " damage in his face, bro.");
            player.GetComponent<PlayerManager>().GetHurt(attackDamage);

            Vector3 dir = player.transform.position - transform.position;
            dir = new Vector3(dir.x, 0, dir.z).normalized * knockbackForce;
            player.GetComponent<PlayerManager>().rb.AddForce(dir, ForceMode.Impulse);
        }
    }
    public void AttackOnCD()
    {
        activeAttackCd = attackDelay;
        attackOnCd = true;
    }

    public void Enable()
    {
        canAttack = true;
        canMove = true;
       
    }
    
    public void Disable()
    {
        canAttack = false;
        canMove = false;
        animator.Play(idleAnimName);
        nav.SetDestination(transform.position);
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
            
            animator.Play(hurtAnimName);
            
            if (currentHealth <= 0)
            {
                Debug.Log("Enemy dead");
                Disable();
                Death();
            }
            
        }
    }

    public void SetSpawnPoint(SpawnArea spawnArea)
    {
        spawnPoint = spawnArea;
    }

    public void PlayAnimation(string anim)
    {
        animator.Play(anim);
    }

    public void Death()
    {
        isDead = true;
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

    public void DebugSetColor(Color newColor)
    {
        GetComponent<MeshRenderer>().material.color = newColor;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dectectionRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void LoadVFX(ParticleSystem effect)
    {
        Instantiate(effect, transform.position, Quaternion.identity);
    }
    
}

