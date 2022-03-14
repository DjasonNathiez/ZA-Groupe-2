using UnityEngine;
using UnityEngine.AI;

public class LionBehaviour : MonoBehaviour
{
    private Animator m_animator;
    private NavMeshAgent m_nav;
    private GameObject m_player;
    
    private AIBrain m_aiBrain;
    
    [Header("State Informations")]
    public StateMachine stateMachine;
    public enum StateMachine{IDLE, CHASE, ATTACK}

    private float distanceToPlayer;
   

    [Header("Attack DATA")] 
    [HideInInspector] public int m_attackDamage;
    [HideInInspector] public float m_attackRange;
    [HideInInspector] public float m_attackDelay;
    [HideInInspector] public float m_activeAttackCD;
    [HideInInspector] public bool canAttack;
    private bool attackOnCD;
    
    private void Awake()
    {
        InitializeData();
    }

    private void InitializeData()
    {
        //Scripts References
        m_animator = GetComponent<Animator>();
        m_aiBrain = GetComponent<AIBrain>();
        m_nav = GetComponent<NavMeshAgent>();
        m_player = FindObjectOfType<PlayerManager>().gameObject;
        
        //Variable References
        m_attackDamage = m_aiBrain.attackDamage;
        m_attackRange = m_aiBrain.attackRange;
        m_attackDelay = m_aiBrain.attackSpeed;
    }

    private void Start()
    {
        m_activeAttackCD = m_attackDelay;
        m_nav.speed = m_aiBrain.moveSpeed;
        m_nav.stoppingDistance = m_aiBrain.attackRange + 0.02f;
    }

    private void Update()
    {
        Detection();
        CheckState();
    }

    #region BRAIN
    
    private void CheckState()
    {
        //SET STATES BY CONDITIONS
        if (m_aiBrain.isAggro)
        {
            stateMachine = distanceToPlayer > m_aiBrain.attackRange +0.02 ? StateMachine.CHASE : StateMachine.ATTACK;
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
        
    }
    
    private void Detection()
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
    
    #endregion
    
    #region ANIMATION EVENTS

    public void DoDamage()
    {
        if (distanceToPlayer < m_aiBrain.attackRange + 0.02)
        {
            Debug.Log("Player take " + m_attackDamage + " damage in his face, bro.");
            m_player.GetComponent<PlayerManager>().GetHurt(m_attackDamage);
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
