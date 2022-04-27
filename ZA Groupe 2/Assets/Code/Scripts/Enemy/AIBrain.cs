using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class AIBrain : MonoBehaviour
{
    public SpawnArea spawnPoint;

    public Rigidbody m_rb;
    //move
    public float moveSpeed;
    
    //health
    public int currentHealth;
    public int maxHealth;

    //attack
    public int attackDamage;
    public float attackRange;
    public float attackSpeed; 
    public float m_attackDelay;
    public float m_activeAttackCD;
    public bool canAttack;
    public bool attackOnCD;
    public float knockbackForce;
    
    //detection
    public float dectectionRange;
    
    //state
    public bool isInvincible;
    public bool isStun;
    public bool isAggro;

    public GameObject m_player;
    public NavMeshAgent m_nav;
    public Animator animator;
    public float distanceToPlayer;

    public bool canFall;
    public bool isFalling;
    public float fallTime;
    public float timeOnGround;
    
    //animations
    public string attackAnimName;
    public string idleAnimName;
    
    //DEBUG
    public Color backupColor;

    private void Update()
    {
        
    }

    public void InitializationData()
    {
        currentHealth = maxHealth;
        
        m_rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_nav = GetComponent<NavMeshAgent>();
    }

    public void ChasePlayer()
    {
        m_nav.SetDestination(m_player.transform.position);
    }
    
    public void Detection()
    {
        distanceToPlayer = Vector3.Distance(transform.position, m_player.transform.position);
        
        Collider[] hit = Physics.OverlapSphere(transform.position, dectectionRange);
        
        foreach (Collider col in hit)
        {
            if (col.GetComponent<PlayerManager>())
            {
                 isAggro = true;
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
        if (attackOnCD)
        {
            AttackCooldown();
        }
        
        if (!attackOnCD)
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
        switch (m_activeAttackCD)
            {
                case > 0:
                    canAttack = false;
                    m_activeAttackCD -= Time.deltaTime;
                    break;
            
                case <= 0:
                    canAttack = true;
                    m_activeAttackCD = m_attackDelay;
                    attackOnCD = false;
                    break;
            }
        
    }
    
    public void DoDamage()
    {
        if (distanceToPlayer < attackRange + 0.02)
        {
            Debug.Log("Player take " + attackRange + " damage in his face, bro.");
            m_player.GetComponent<PlayerManager>().GetHurt(attackDamage);

            Vector3 dir = m_player.transform.position - transform.position;
            dir = new Vector3(dir.x, 0, dir.z).normalized * knockbackForce;
            m_player.GetComponent<PlayerManager>().m_rb.AddForce(dir, ForceMode.Impulse);
        }
    }
    public void AttackOnCD()
    {
        m_activeAttackCD = m_attackDelay;
        attackOnCD = true;
    }
    
    public void GetHurt(int damage)
    {
        if (!isInvincible)
        {
            switch (currentHealth)
            {
                case > 0:
                    currentHealth -= damage;
                    break;
            }
            
        }
    }

    public void SetSpawnPoint(SpawnArea spawnArea)
    {
        spawnPoint = spawnArea;
    }

    public IEnumerator Death()
    {
        yield return new WaitForSeconds(0.1f);
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
    
}

