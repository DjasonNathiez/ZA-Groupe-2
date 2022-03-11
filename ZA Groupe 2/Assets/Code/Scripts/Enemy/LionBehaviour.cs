using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LionBehaviour : MonoBehaviour
{
    private Animator m_animator;
    private NavMeshAgent m_nav;
    private GameObject m_player;
    //retirer le Serialize quand fini
    [SerializeField] private AIBrain m_aiBrain;
    
    public StateMachine stateMachine;
    public enum StateMachine{IDLE, CHASE, ATTACK}

    private float distanceToPlayer;
    private bool isAggro;
    
    private void Awake()
    {
        m_animator = GetComponent<Animator>();
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
        if (isAggro)
        {
            stateMachine = distanceToPlayer > m_aiBrain.attackRange ? StateMachine.CHASE : StateMachine.ATTACK;
        }
        
        Detection();
        CheckState();
    }

    private void CheckState()
    {
        switch (stateMachine)
        {
            case StateMachine.IDLE:
                m_animator.Play("ennemy_lion_idle");
                break;
            
            case StateMachine.CHASE:
                
                m_animator.Play("ennemy_lion_idle");
                
                ChasePlayer();
                break;
            
            case StateMachine.ATTACK:
                
                m_animator.Play("ennemy_lion_attack");
                
                AttackPlayer();
                
                break;
        }
    }

    private void AttackPlayer()
    {
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
                isAggro = true;
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
