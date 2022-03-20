using UnityEngine;
using UnityEngine.AI;

public class LionBehaviour : AIBrain
{
    private Animator m_animator;
    
    [Header("State Informations")]
    public StateMachine stateMachine;
    public enum StateMachine{IDLE, CHASE, ATTACK}

    [HideInInspector] public float m_attackDelay;
    [HideInInspector] public float m_activeAttackCD;
    [HideInInspector] public bool canAttack;
    private bool attackOnCD;

    void Awake()
    {
        InitializationData();
        m_animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_activeAttackCD = m_attackDelay;
        m_nav.speed = moveSpeed;
        m_nav.stoppingDistance = attackRange + 0.02f;
    }

    private void Update()
    {
        CheckState();
        Detection();
    }

    private void CheckState()
    {
        //SET STATES BY CONDITIONS
        if (isAggro)
        {
            stateMachine = distanceToPlayer > attackRange +0.02 ? StateMachine.CHASE : StateMachine.ATTACK;
        }
        
        //APPLY STATES ACTION
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
                AttackPlayer();
                break;
        }
        
        if (currentHealth <= 0)
        {
            Death();
        }
        
    }

    #region ANIMATION EVENTS

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

    #endregion

    #region BEHAVIOUR

    #region MOVEMENT

    private void ChasePlayer()
    {
        m_nav.SetDestination(m_player.transform.position);
    }

    #endregion


    #region COMBAT

    private void AttackPlayer()
    {
        if (!attackOnCD)
        {
            m_animator.Play("ennemy_lion_attack");
        }
        AttackCooldown();
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

    #endregion
    
    #endregion
    
}
