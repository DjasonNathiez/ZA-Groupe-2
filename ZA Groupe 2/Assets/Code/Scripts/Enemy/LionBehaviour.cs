using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LionBehaviour : MonoBehaviour
{
    private NavMeshAgent m_nav;
    private GameObject m_player;
    //retirer le Serialize quand fini
    [SerializeField] private AIBrain m_aiBrain;
    
    public StateMachine stateMachine;
    public enum StateMachine{IDLE, CHASE, ATTACK}

    private float distanceToPlayer;

    private Vector3 m_goToPoint;
    
    private void Awake()
    {
        
        m_aiBrain = GetComponent<AIBrain>();
        m_nav = GetComponent<NavMeshAgent>();
        m_player = FindObjectOfType<PlayerManager>().gameObject;
        

    }

    private void Start()
    {
        m_nav.speed = m_aiBrain.moveSpeed;
        m_nav.stoppingDistance = m_aiBrain.attackRange;
        
        SetNav();
    }

    private void Update()
    {
        Detection();
        CheckState();
    }

    private void CheckState()
    {
        switch (stateMachine)
        {
            case StateMachine.IDLE:
                TravelToPoint();
                break;
            
            case StateMachine.CHASE:
                
                if (distanceToPlayer > m_aiBrain.attackRange)
                {
                    ChasePlayer();
                    
                }
                else
                {
                    stateMachine = StateMachine.ATTACK;
                }
                
                break;
            
            case StateMachine.ATTACK:
                break;
        }
    }

    private void AttackPlayer()
    {
        Debug.Log("Attack");
    }
    
    private void SetNav()
    {
        m_aiBrain.spawnPoint.GeneratePositionInArea();
        
        m_goToPoint = m_aiBrain.spawnPoint.pathPoint;
        
        Debug.Log("New point is " + m_goToPoint);
    }

    private void TravelToPoint()
    {
        var distanceToPoint = Vector3.Distance(transform.position, m_goToPoint);

        switch (distanceToPoint)
        {
            case >= 0.5f:
                m_nav.SetDestination(m_goToPoint);
                break;
            
            case <= 0.5f:
                Debug.Log(this.gameObject.name + " is on point.");
                SetNav();
                break;
        }
    }
    

    private void ChasePlayer()
    {
        m_nav.SetDestination(m_player.transform.position);
    }
    
    private void Detection()
    {
        distanceToPlayer = Vector3.Distance(transform.position, m_player.transform.position);
        
        Collider[] hit = Physics.OverlapSphere(transform.position, m_aiBrain.dectectionRange);
        foreach (Collider col in hit)
        {
            if (col.GetComponent<PlayerManager>())
            {
                stateMachine = StateMachine.CHASE;
            }
        }
        
    }

    private void OnDrawGizmos()
    {
        if (m_aiBrain)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_aiBrain.dectectionRange);
        }
    }
}
