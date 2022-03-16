using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BearBehaviour : MonoBehaviour
{
    private NavMeshAgent m_nav;
    private AIBrain m_aiBrain;
    
    public StateMachine stateMachine;
    public enum StateMachine{IDLE, CHASE, ATTACK, ONGROUND}

    private GameObject m_player;
    private float distanceToPlayer;

    private void OnValidate()
    {
        InitializationData();
    }

    private void Update()
    {
        CheckState();
        Detection();
    }

    void Chase()
    {
        m_nav.SetDestination(m_player.transform.position);
    }

    void InitializationData()
    {
        m_aiBrain = GetComponent<AIBrain>();
        m_nav = GetComponent<NavMeshAgent>();
        m_player = GameObject.FindGameObjectWithTag("Player");
    }

    void CheckState()
    {
        if (m_aiBrain.isAggro)
        {
            stateMachine = distanceToPlayer > m_aiBrain.attackRange +0.02 ? StateMachine.CHASE : StateMachine.ATTACK;
        }
        
        switch (stateMachine)
        {
            case StateMachine.CHASE:
                Chase();
                break;
        }
    }
    
    void Detection()
    {
        distanceToPlayer = Vector3.Distance(transform.position, m_player.transform.position);
        
        Collider[] hit = Physics.OverlapSphere(transform.position, m_aiBrain.dectectionRange);
        
        foreach (Collider col in hit)
        {
            if (col.GetComponent<PlayerManager>())
            {
                m_aiBrain.isAggro = true;
            }

            if (col.GetComponent<AIBrain>())
            {
                var colEnemy = col.GetComponent<AIBrain>();

                if (colEnemy.isAggro)
                {
                    m_aiBrain.isAggro = true;
                }
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        if (m_aiBrain)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_aiBrain.dectectionRange);
            Gizmos.DrawWireSphere(transform.position, m_aiBrain.attackRange);
        }
    }
}
