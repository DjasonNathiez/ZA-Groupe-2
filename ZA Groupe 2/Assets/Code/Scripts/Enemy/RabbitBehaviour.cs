using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitBehaviour : AIBrain
{
    public StateMachine stateMachine;
    public enum StateMachine{IDLE, CHASE, ATTACK}

    public Vector3[] detectedPoints;
    public List<float> distanceToPoints;
    
    // Start is called before the first frame update
    void Start()
    {
        InitializationData();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_player.GetComponent<PlayerManager>().m_rope.enabled)
        {
            detectedPoints = m_player.GetComponent<TestRope>().CalculateCuttingPoints(1);

            if (detectedPoints.Length > distanceToPoints.Count)
            {
                AddPointDistanceToRope();
            }

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
}
