using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitBehaviour : AIBrain
{
    public StateMachine stateMachine;
    public enum StateMachine{IDLE, CHASE, ATTACK}

    public Vector3[] detectedPoints;
    public List<float> distanceToPoints;
    public Vector3 nearestPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        InitializationData();
    }

    // Update is called once per frame
    void Update()
    {
        RopePointDetection();
        CheckState();
    }

    private void CheckState()
    {
        if (currentHealth <= 0)
        {
            Death();
        }
        
        float distanceToNearestPoint = Vector3.Distance(transform.position, nearestPoint);
        
        if (m_player.GetComponent<PlayerManager>().m_rope.enabled)
        {
            if (dectectionRange > distanceToNearestPoint)
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
                break;
            
            case StateMachine.CHASE:
                MoveToRope();
                break;
            
            case StateMachine.ATTACK:
                m_player.GetComponent<PlayerManager>().m_rope.rewinding = true;
                break;
        }

        isInvincible = !isAggro;
    }

    private void MoveToRope()
    {
        m_nav.SetDestination(nearestPoint);
    }

    private void RopePointDetection()
    {
        if (m_player.GetComponent<PlayerManager>().m_rope.enabled)
        {
            detectedPoints = m_player.GetComponent<TestRope>().CalculateCuttingPoints(1);

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
