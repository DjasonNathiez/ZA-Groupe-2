using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class AIBrain : MonoBehaviour
{
    [SerializeField] private AIData aiData;
    public SpawnArea spawnPoint;
    
    //move
    public float moveSpeed;
    
    //health
    public int currentHealth;
    public int maxHealth;

    //attack
    public int attackDamage;
    public float attackRange;
    public float attackSpeed;
    [HideInInspector] public float m_attackDelay;
    [HideInInspector] public float m_activeAttackCD;
    [HideInInspector] public bool canAttack;
    public bool attackOnCD;
    
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

    //DEBUG
    public Color backupColor;

    public void InitializationData()
    {
        maxHealth = aiData.health;
        attackDamage = aiData.attackDamage;
        attackSpeed = aiData.attackSpeed;
        attackRange = aiData.attackRange;
        moveSpeed = aiData.moveSpeed;
        dectectionRange = aiData.detectionRange;
        currentHealth = maxHealth;
        
        backupColor = GetComponent<MeshRenderer>().material.color;


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
        if (!attackOnCD && attackAnimName != String.Empty)
        {
            animator.Play(attackAnimName);
        }
        AttackCooldown();
        
        //debug console
        if (attackAnimName == String.Empty)
        {
            Debug.Log("There is no anim name attach to the attack action, check this in inspector");
        }
    }
    
    private void AttackCooldown()
    {
        if (attackOnCD)
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
    }
    
    public void DoDamage()
    {
        if (distanceToPlayer < attackRange + 0.02)
        {
            Debug.Log("Player take " + attackRange + " damage in his face, bro.");
            m_player.GetComponent<PlayerManager>().GetHurt(attackDamage);
        }
    }
    public void AttackOnCD()
    {
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
            
            StartCoroutine(TiltColorDebug());
        }
    }

    IEnumerator TiltColorDebug()
    {
        DebugSetColor(Color.red);
        yield return new WaitForSeconds(0.1f);
        DebugSetColor(backupColor);
    }

    public void SetSpawnPoint(SpawnArea spawnArea)
    {
        spawnPoint = spawnArea;
    }

    public void Death()
    {
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

