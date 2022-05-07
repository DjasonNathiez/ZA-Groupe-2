using System.Collections.Generic;
using UnityEngine;

public class RabbitBehaviour : AIBrain
{
    public StateMachine stateMachine;
    public enum StateMachine{IDLE, CHASE, ATTACK}

    public Vector3[] detectedPoints;
    public List<float> distanceToPoints;
    public Vector3 nearestPoint;

    public float areaToMove;
    private Vector3 pointToGo;
    public float avoidFront;
    private Vector3 originPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        originPoint = transform.position;
        
        float minX = transform.position.x - areaToMove;
        float minZ = transform.position.z - areaToMove;
        float maxX = transform.position.x + areaToMove;
        float maxZ = transform.position.z + areaToMove;

        pointToGo = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minZ, maxZ));
        
        InitializationData();
    }

    // Update is called once per frame
    void Update()
    {

        CheckState();
        Detection();
    }

    private void CheckState()
    {
        if (currentHealth <= 0)
        {
            StartCoroutine(Death());
        }
        
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
                animator.Play("R_Idle");
                
                nav.SetDestination(pointToGo);

                Vector3 pointToGoMin = new Vector3(pointToGo.x - 3, transform.position.y, pointToGo.z - 3);
                Vector3 pointToGoMax = new Vector3(pointToGo.x + 3, transform.position.y, pointToGo.z + 3);

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
                animator.Play("R_Chase");
                MoveToRope();
                break;
            
            case StateMachine.ATTACK:
                animator.Play(attackAnimName);
                player.GetComponent<PlayerManager>().rope.rewinding = true;
                break;
        }
    }

    private void SetNavPoint()
    {
      
        float minX = originPoint.x - areaToMove;
        float minZ = originPoint.z - areaToMove;
        float maxX = originPoint.x + areaToMove;
        float maxZ = originPoint.z + areaToMove;

        pointToGo = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minZ, maxZ));
        Debug.Log(pointToGo);
        
    }

    private void MoveToRope()
    {
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
