using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitBehaviour : AIBrain
{
    [Header("Rabbit Self Data")]
    
    public float areaToMove;
    private Vector3 m_pointToGo;
    public float avoidFront;
    private Vector3 m_originPoint;
    public float timeToGoNextPoint;
    
    [HideInInspector] public StateMachine stateMachine;
    public enum StateMachine{IDLE, CHASE, ATTACK}

    public bool isPatrolling;

    [HideInInspector] public Vector3[] detectedPoints;
    [HideInInspector] public List<float> distanceToPoints;
    [HideInInspector] public Vector3 nearestPoint;

    void Start()
    {
        isInvincible = false;
        m_originPoint = transform.position;
        
        float minX = transform.position.x - areaToMove;
        float minZ = transform.position.z - areaToMove;
        float maxX = transform.position.x + areaToMove;
        float maxZ = transform.position.z + areaToMove;

        m_pointToGo = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minZ, maxZ));
        
        InitializationData();
    }

    void Update()
    {
        //Animator Set Bool
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isPatrolling", isPatrolling);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isHurt", isHurt);
        animator.SetBool("isDead", isDead);
        
        if (!isDead)
        {
            CheckState();
            Detection();
            SetColor();
        }
        
        enemyStatusPointer.GetComponent<MeshRenderer>().material = isAggro ? aggroMaterial : nonAggroMaterial;
    }

    private void CheckState()
    {
        float distanceToNearestPoint = Vector3.Distance(transform.position, nearestPoint);
        
        if (player.GetComponent<PlayerManager>().rope.enabled && distanceToPlayer <= dectectionRange)
        {
            RopePointDetection();
            
            if (dectectionRange < distanceToNearestPoint)
            {
                stateMachine = StateMachine.CHASE;
                
                isAggro = true;
            }
            
            stateMachine = attackRange > distanceToNearestPoint ? StateMachine.ATTACK : StateMachine.CHASE;
        }
        else
        {
            stateMachine = StateMachine.IDLE;
            isAggro = false;
        }

        switch (stateMachine)
        {
            case StateMachine.IDLE:
                isAttacking = false;
                isPatrolling = true;
                isMoving = false;
                isAggro = false;
                
                nav.SetDestination(m_pointToGo);
                
                
                Vector3 pointToGoMin = new Vector3(m_pointToGo.x - 3, transform.position.y, m_pointToGo.z - 3);
                Vector3 pointToGoMax = new Vector3(m_pointToGo.x + 3, transform.position.y, m_pointToGo.z + 3);

                if (transform.position.x >= pointToGoMin.x && transform.position.z >= pointToGoMin.z &&
                    transform.position.x <= pointToGoMax.x && transform.position.z <= pointToGoMax.z)
                {
                    SetNavPoint();
                }

                RaycastHit hit; 
                if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
                {
                    float distToFrontHit = Vector3.Distance(transform.forward, hit.point);
                            
                    if (distToFrontHit <= avoidFront)
                    {
                        SetNavPoint();
                    }
                }
                
                break;

            case StateMachine.CHASE:
                isAggro = true;
                isPatrolling = false;
                isAttacking = false;
                MoveToRope();
                break;
            
            case StateMachine.ATTACK:
                isAttacking = true;
                player.GetComponent<PlayerManager>().rope.rewinding = true;
                break;
        }
    }

    private void SetNavPoint()
    {
      
        float minX = m_originPoint.x - areaToMove;
        float minZ = m_originPoint.z - areaToMove;
        float maxX = m_originPoint.x + areaToMove;
        float maxZ = m_originPoint.z + areaToMove;

        m_pointToGo = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minZ, maxZ));
        
    }

    private void MoveToRope()
    {
        isMoving = true;
        nav.SetDestination(nearestPoint);
    }

    private void RopePointDetection()
    {
        if (player.GetComponent<PlayerManager>().rope.enabled)
        {
            detectedPoints = player.GetComponent<Rope>().CalculateCuttingPoints(1);

            if (detectedPoints.Length > distanceToPoints.Count)
            {
                AddPointDistanceToRope();
            }
            
            
            if (distanceToPoints.Count > 0)
            {
                CheckNearestPoint();
            }
        }
        else
        {
            nearestPoint = Vector3.zero;
        }

        if (distanceToPoints != null && detectedPoints != null)
        {
            if (distanceToPoints.Count > detectedPoints.Length)
            {
                foreach (float f in distanceToPoints)
                {
                    for (int i = detectedPoints.Length; i < distanceToPoints.Count; i++)
                    {
                        distanceToPoints.Remove(f);
                    }
                }
            }
        }
        

    }

    private void AddPointDistanceToRope()
    {
        for (int i = distanceToPoints.Count; i < detectedPoints.Length; i++)
        {
            distanceToPoints.Add(Vector3.Distance(transform.position, detectedPoints[i]));
        }
    }

    private void CheckNearestPoint()
    {
        for (int i = 0; i < distanceToPoints.Count - 1 ; i++)
        {
            if (distanceToPoints[i] < distanceToPoints[i + 1])
            {
                nearestPoint = detectedPoints[i];
            }
        }
    }
}
