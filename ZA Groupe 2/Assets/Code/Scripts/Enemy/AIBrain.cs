using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class AIBrain : MonoBehaviour
{
    public SpawnArea spawnPoint;

    [FormerlySerializedAs("m_rb")] public Rigidbody rb;
    //move
    public float moveSpeed;
    
    //health
    public int currentHealth;
    public int maxHealth;

    //attack
    public int attackDamage;
    public float attackRange;
    public float attackSpeed; 
    [FormerlySerializedAs("m_attackDelay")] public float attackDelay;
    [FormerlySerializedAs("m_activeAttackCD")] public float activeAttackCd;
    public bool canAttack;
    [FormerlySerializedAs("attackOnCD")] public bool attackOnCd;
    public float knockbackForce;
    
    //detection
    public float dectectionRange;
    
    //state
    public bool isInvincible;
    public bool haveCounterState;
    public bool isStun;
    public bool isAggro;
    public bool canMove;

    [FormerlySerializedAs("m_player")] public GameObject player;
    [FormerlySerializedAs("m_nav")] public NavMeshAgent nav;
    public Animator animator;
    public float distanceToPlayer;

    public bool canFall;
    public bool isFalling;
    public float fallTime;
    public float timeOnGround;

    public bool isDead;
    
    //animations
    public string attackAnimName;
    public string idleAnimName;
    
    //DEBUG
    public Color backupColor;

    public ParticleSystem hurtVFX;

    public bool canKnockback;
    

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
        }
        else
        {
            animator.Play(idleAnimName);
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
        if (distanceToPlayer < attackRange + 0.02)
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
    
    public void GetHurt(int damage)
    {
        if (!isInvincible)
        {
            switch (currentHealth)
            {
                case > 0:
                    currentHealth -= damage;
                    if (hurtVFX != null)
                    {
                        hurtVFX.Play();
                    }
                    
                    // if (haveCounterState) isInvincible = true;
                    
                    break;
            }
            
        }
        else
        {
            //Play Anim Counter
            //Play VFX Counter
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
        //yield return new WaitForSeconds(0.1f);
       // GameManager.instance.DropItem("Popcorn", transform);
       // Destroy(gameObject);
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

