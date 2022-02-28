using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRope : MonoBehaviour
{
    public GameObject pin;
    public LineRenderer rope;
    public float checkDistance;
    public float currentAngle = 0;
    private Vector3 previousVector3;
    public List<Node> nodes = new List<Node>();
    public int movingstate;

    void Update()
    {
        rope.SetPosition(rope.positionCount-1,transform.position-rope.transform.position);
        Vector3 point = rope.GetPosition(rope.positionCount - 2) + rope.transform.position;
        Vector3 dir = (point - transform.position);
        currentAngle += Vector3.SignedAngle(previousVector3, dir,Vector3.up);
        if (Vector3.SignedAngle(previousVector3, dir, Vector3.up) > 0)
        {
            movingstate = 1;
        }
        else if (Vector3.SignedAngle(previousVector3, dir, Vector3.up) < 0)
        {
            movingstate = -1;
        }
        else
        {
            movingstate = 0;
        }
        previousVector3 = dir;
        Ray ray = new Ray(transform.position, dir);
        if (Physics.Raycast(ray, out RaycastHit hit, Vector3.Distance(transform.position, point)-checkDistance))
        {
            if (Vector3.Distance(hit.point, rope.GetPosition(rope.positionCount - 2) + rope.transform.position) > checkDistance)
            {
                rope.positionCount += 1;
                rope.SetPosition(rope.positionCount-2,hit.point-rope.transform.position);
                Node nodeToCreate = new Node();
                nodeToCreate.angle = currentAngle;
                if (movingstate == 1)
                {
                    nodeToCreate.positive = true;   
                }
                else if (movingstate == -1)
                {
                    nodeToCreate.positive = false;   
                }
                nodes.Add(nodeToCreate);
                rope.SetPosition(rope.positionCount-1,transform.position-rope.transform.position);
            }
        }

        if (nodes.Count > 0 && nodes[nodes.Count - 1].positive)
        {
            if (currentAngle < nodes[nodes.Count - 1].angle)
            {
                nodes.RemoveAt(nodes.Count-1);
                rope.positionCount -= 1;
                rope.SetPosition(rope.positionCount-1,transform.position-rope.transform.position);
            }
        }
        else if (nodes.Count > 0 && !nodes[nodes.Count - 1].positive)
        {
            if (currentAngle > nodes[nodes.Count - 1].angle)
            {
                nodes.RemoveAt(nodes.Count-1);
                rope.positionCount -= 1;
                rope.SetPosition(rope.positionCount-1,transform.position-rope.transform.position);
            }
        }
    }

    
}

[Serializable] public class Node
{
    public float angle;
    public bool positive;
}
