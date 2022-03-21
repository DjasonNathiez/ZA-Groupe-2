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
    
    //detection
    public float dectectionRange;
    
    //state
    public bool isInvincible;
    public bool isStun;
    public bool isAggro;

    public GameObject m_player;
    public NavMeshAgent m_nav;
    public float distanceToPlayer;


    public bool canFall;
    public bool isFalling;

    public void InitializationData()
    {
        maxHealth = aiData.health;
        attackDamage = aiData.attackDamage;
        attackSpeed = aiData.attackSpeed;
        attackRange = aiData.attackRange;
        moveSpeed = aiData.moveSpeed;
        dectectionRange = aiData.detectionRange;
        currentHealth = maxHealth;
        
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_nav = GetComponent<NavMeshAgent>();
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
        var backupColor = GetComponent<MeshRenderer>().material.color;
        
        GetComponent<MeshRenderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<MeshRenderer>().material.color = backupColor;
    }

    public void SetSpawnPoint(SpawnArea spawnArea)
    {
        spawnPoint = spawnArea;
    }

    public void Death()
    {
        Destroy(gameObject);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dectectionRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    
}

