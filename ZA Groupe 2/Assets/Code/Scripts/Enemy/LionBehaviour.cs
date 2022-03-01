using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LionBehaviour : MonoBehaviour
{
    private NavMeshAgent m_nav;
    private GameObject m_player;
    private AIBrain m_aiBrain;
    
    public StateMachine stateMachine;
    public enum StateMachine{IDLE, CHASE, ATTACK}

    private float distanceToPlayer;
    
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
                //do nothing
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
