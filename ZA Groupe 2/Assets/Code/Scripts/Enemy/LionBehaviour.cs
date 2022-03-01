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

    private void Awake()
    {
        m_aiBrain = GetComponent<AIBrain>();
        m_nav = GetComponent<NavMeshAgent>();
        m_player = FindObjectOfType<PlayerManager>().gameObject;
    }

    private void Update()
    {
        Detection();
    }

    private void CheckState()
    {
       
    }

    private void Detection()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, m_aiBrain.dectectionRange);
        foreach (Collider col in hit)
        {
            if (col.GetComponent<PlayerManager>())
            {
                Debug.Log("Player in contact");
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
