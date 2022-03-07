using System;
using System.Collections.Generic;
using UnityEngine;

public class TestRope : MonoBehaviour
{
    public GameObject pin;
    public bool pinnedToObject;
    public Rigidbody pinnedRb;
    public float pinnedObjectDistance;
    public LineRenderer rope;
    public float checkDistance;
    public List<Node> nodes = new List<Node>();
    public float borderDist = 0.05f;
    public float maximumLenght;
    public float lenght;
    public bool electrocuted;
    public bool rewinding;
    public float remainingLenght;
    public GameObject nodePos;
    public PlayerManager PlayerManager;

    void Update()
    {
        // CHECK DE NOUVELLES COLLISIONS AVEC LE DERNIER SEGMENT
        Vector3 point;
        if (nodes.Count > 0)
        {
            point = nodes[nodes.Count - 1].nodePoint.transform.position;
        }
        else
        {
            point = pin.transform.position;
        }
        Vector3 dir = (point - transform.position);
        Ray ray = new Ray(transform.position, dir);
        if (Physics.Raycast(ray, out RaycastHit hit, Vector3.Distance(transform.position, point)))
        {
            
                rope.positionCount += 1;
                Node nodeToCreate = new Node();
                nodeToCreate.index = rope.positionCount - 2;
                nodeToCreate.anchor = hit.transform.gameObject;
                Vector3 pos = hit.collider.ClosestPoint(hit.point) + (hit.point - hit.transform.position).normalized * borderDist;
                nodeToCreate.nodePoint = Instantiate(nodePos, new Vector3(pos.x, transform.position.y, pos.z), Quaternion.identity, nodeToCreate.anchor.transform);
                nodeToCreate.nodePoint.name = "Node " + nodeToCreate.index;
                if(Vector3.SignedAngle(nodeToCreate.anchor.transform.position-transform.position,dir,Vector3.up) > 0)
                {
                    nodeToCreate.positive = true;
                }
                else
                {
                    nodeToCreate.positive = false;   
                }
                nodes.Add(nodeToCreate);
                CheckElectrocution();
            
        }
        //Debug.DrawRay(ray.origin + Vector3.up,ray.direction * Vector3.Distance(transform.position, point),Color.red);

        // CHECK DE NOUVELLES COLLISIONS AVEC LE PREMIER SEGMENT
        
        if (nodes.Count > 0)
        {
            dir = pin.transform.position - (nodes[0].nodePoint.transform.position);
            ray = new Ray((nodes[0].nodePoint.transform.position)+dir.normalized*checkDistance, dir);
            if (Physics.Raycast(ray, out hit, dir.magnitude-checkDistance))
            {
                Vector3 pos = hit.collider.ClosestPoint(hit.point) + (hit.point - hit.transform.position).normalized * borderDist;
                if (Vector3.Distance(pos, pin.transform.position) > checkDistance &&
                    Vector3.Distance(pos, ray.origin) > checkDistance)
                {
                    rope.positionCount += 1;
                    Node nodeToCreate = new Node();
                    nodeToCreate.index = 1;
                    nodeToCreate.anchor = hit.transform.gameObject;
                    nodeToCreate.nodePoint = Instantiate(nodePos, new Vector3(pos.x, transform.position.y, pos.z), Quaternion.identity, nodeToCreate.anchor.transform);
                    nodeToCreate.nodePoint.name = "Node " + nodeToCreate.index;
                    float signedAngle = Vector3.SignedAngle(
                        nodeToCreate.anchor.transform.position+rope.transform.position - (nodes[0].nodePoint.transform.position + rope.transform.position),
                        dir, Vector3.up);
                    if( signedAngle > 0)
                    {
                        nodeToCreate.positive = true;
                    }
                    else
                    {
                        nodeToCreate.positive = false;   
                    }
                    foreach (Node nodeToFix in nodes)
                    {
                        nodeToFix.index += 1;
                        rope.SetPosition(nodeToFix.index,nodeToFix.nodePoint.transform.position);
                    }
                    nodes.Insert(0,nodeToCreate);
                    CheckElectrocution();
                }
            }  
            //Debug.DrawRay(ray.origin + Vector3.up,ray.direction * (dir.magnitude-checkDistance),Color.blue);
        }

        // CHECK DE COLLISIONS AVEC TOUS LES AUTRES NODES
        
        foreach (Node node in nodes)
        {
            if (node.index > 1 && nodes[node.index - 2].anchor != node.anchor)
            {
                Vector3 dirNode = nodes[node.index - 2].nodePoint.transform.position - node.nodePoint.transform.position;
                Ray rayNode = new Ray((node.nodePoint.transform.position)+dirNode.normalized*checkDistance, dirNode);
                if (Physics.Raycast(rayNode, out RaycastHit hitNode, Vector3.Distance(nodes[node.index - 2].nodePoint.transform.position, node.nodePoint.transform.position)))
                {
                    Vector3 pos = hitNode.collider.ClosestPoint(hitNode.point) + (hitNode.point - hitNode.transform.position).normalized * borderDist;
                    if (Vector3.Distance(pos,node.nodePoint.transform.position) > checkDistance && Vector3.Distance(pos,nodes[node.index - 2].nodePoint.transform.position) > checkDistance)
                    {
                        rope.positionCount += 1;

                        Node nodeToCreate = new Node();
                        nodeToCreate.index = node.index;
                        nodeToCreate.anchor = hitNode.transform.gameObject;
                        nodeToCreate.nodePoint = Instantiate(nodePos, new Vector3(pos.x, transform.position.y, pos.z), Quaternion.identity, nodeToCreate.anchor.transform);
                        nodeToCreate.nodePoint.name = "Node " + nodeToCreate.index;
                        float signedAngle = Vector3.SignedAngle(
                            nodeToCreate.anchor.transform.position - (nodes[node.index - 1].nodePoint.transform.position),
                            dirNode, Vector3.up);
                        if( signedAngle > 0)
                        {
                            nodeToCreate.positive = true;
                        }
                        else
                        {
                            nodeToCreate.positive = false;   
                        }
                        foreach (Node nodeToFix in nodes)
                        {
                            if (nodeToFix.index >= node.index)
                            {
                                nodeToFix.index += 1;
                            }
                            rope.SetPosition(nodeToFix.index,nodeToFix.nodePoint.transform.position);
                        }
                        nodes.Insert(node.index-2,nodeToCreate);
                        CheckElectrocution();
                        break;   
                    }
                }
                //Debug.DrawRay(rayNode.origin+Vector3.up,rayNode.direction*Vector3.Distance(nodes[node.index - 2].nodePoint.transform.position, node.nodePoint.transform.position),Color.green);
            }
        }

        Debug.Log("OK");
        rope.SetPosition(rope.positionCount-1,transform.position-rope.transform.position);
        rope.SetPosition(0,pin.transform.position-rope.transform.position);
        foreach (Node node in nodes)
        {
            rope.SetPosition(node.index,node.nodePoint.transform.position-rope.transform.position);
        }
        Debug.Log("LEZGO");
        
        // SUPPRESSION DES NODES
        
        if (nodes.Count > 0)
        {
            int index = rope.positionCount;
            for (int i = index; i > 2; i--)
            {
                float angleDiffNode = Vector3.SignedAngle((rope.GetPosition(i - 1)-rope.GetPosition(i - 2)).normalized,(rope.GetPosition(i - 2)-rope.GetPosition(i - 3)).normalized, Vector3.up);
 
                if (nodes[i-3].positive)
                {
                    if (angleDiffNode > 0)
                    {
                        for (int N = i-2; N < rope.positionCount-1; N++)
                        {
                            rope.SetPosition(N,rope.GetPosition(N+1));
                        }
                        Destroy(nodes[i-3].nodePoint);
                        nodes.RemoveAt(i-3);
                        foreach (Node node in nodes)
                        {
                            if (node.index > i - 2)
                            {
                                node.index -= 1;
                            }
                        }
                        rope.positionCount -= 1;
                        rope.SetPosition(rope.positionCount-1,transform.position-rope.transform.position);
                        CheckElectrocution();
                    }
                }
                else
                {
                    if (angleDiffNode < 0)
                    {
                        for (int N = i-2; N < rope.positionCount-1; N++)
                        {
                            rope.SetPosition(N,rope.GetPosition(N+1));
                        }
                        Destroy(nodes[i-3].nodePoint);
                        nodes.RemoveAt(i-3);
                        foreach (Node node in nodes)
                        {
                            if (node.index > i - 2)
                            {
                                node.index -= 1;
                            }
                        }
                        rope.positionCount -= 1;
                        rope.SetPosition(rope.positionCount-1,transform.position-rope.transform.position);
                        CheckElectrocution();
                    }
                }
            }
        }
        Debug.Log("LEZGONG");
        // AUTRES TRUCS DE PHYSIQUE DE TIRAGE D'OBJET

        float checklenght = 0;
        for (int i = 1; i < rope.positionCount; i++)
        {
            checklenght += Vector3.Distance(rope.GetPosition(i), rope.GetPosition(i - 1));
        }
        lenght = checklenght;
        
        remainingLenght = maximumLenght - (checklenght - Vector3.Distance(rope.GetPosition(rope.positionCount-2), rope.GetPosition(rope.positionCount-1)));;

        if (pinnedToObject)
        {
            if (lenght > pinnedObjectDistance)
            {
                Vector3 force = (rope.GetPosition(1) - rope.GetPosition(0)).normalized*0.01f* (lenght - pinnedObjectDistance);
                pinnedRb.AddForceAtPosition(force,pin.transform.position,ForceMode.VelocityChange);
            }
        }
        
        if (Vector3.Distance(rope.GetPosition(rope.positionCount-2), rope.GetPosition(rope.positionCount-1)) > remainingLenght)
        {
            if (pinnedToObject)
            {
                pinnedRb.AddForceAtPosition((rope.GetPosition(1) - rope.GetPosition(0)).normalized*0.01f*GetComponent<PlayerManager>().m_rb.velocity.magnitude,pin.transform.position,ForceMode.VelocityChange);
            }
            Vector3 newPos = (rope.GetPosition(rope.positionCount - 2)+rope.transform.position) + (transform.position - (rope.GetPosition(rope.positionCount - 2)+rope.transform.position)).normalized * remainingLenght;
            transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
        }

        // REWINDING DU CABLE
        
        if (rewinding)
        {
            if (nodes.Count > 0)
            {
                float newlenght = 50;
                int check = nodes.Count;
                for (int i = 0; i < check; i++)
                {
                    Debug.Log("ATTENTION , i = " + i);
                        if (((nodes[0].nodePoint.transform.position - pin.transform.position).normalized * Time.deltaTime * newlenght).magnitude <
                            (nodes[0].nodePoint.transform.position - pin.transform.position).magnitude)
                        {
                            Vector3 newPos = pin.transform.position + (nodes[0].nodePoint.transform.position - pin.transform.position).normalized* Time.deltaTime * newlenght;
                            pin.transform.position = newPos;
                            break;
                        }
                        else
                        {
                            newlenght = ((nodes[0].nodePoint.transform.position - pin.transform.position).normalized * Time.deltaTime * newlenght).magnitude - (nodes[0].nodePoint.transform.position - pin.transform.position).magnitude;
                            Debug.Log(newlenght + " et i = " + i);
                            pin.transform.position = nodes[0].nodePoint.transform.position;
                            for (int N = 1; N < rope.positionCount-1; N++)
                            {
                                rope.SetPosition(N,rope.GetPosition(N+1));
                            }
                            Destroy(nodes[0].nodePoint);
                            nodes.RemoveAt(0);
                            foreach (Node node in nodes)
                            {
                                if (node.index > 0)
                                {
                                    node.index -= 1;
                                }
                            }
                            rope.positionCount -= 1;
                            rope.SetPosition(rope.positionCount-1,transform.position-rope.transform.position);
                        }
                    
                }
            }
            else
            {
                if (((transform.position - pin.transform.position).normalized * Time.deltaTime * 50).magnitude <
                    (transform.position - pin.transform.position).magnitude)
                {
                    Vector3 newPos = pin.transform.position + (transform.position - pin.transform.position).normalized* Time.deltaTime * 50;
                    pin.transform.position = newPos;
                }
                else
                {
                    pin.SetActive(false);
                    Debug.Log("A la fin du Rewind mais avant, state = " + PlayerManager.state);
                    PlayerManager.state = "StatusQuo";
                    Debug.Log("A la fin du Rewind , state = " + PlayerManager.state);
                    rope.gameObject.SetActive(false);
                    pinnedToObject = false;
                    rewinding = false;
                    enabled = false;
                }
            }
        }
    }
    void CheckElectrocution()
    {
        bool checkedElectrocution = false;
        foreach (Node node in nodes)
        {
            ElectrocutedProp electrocutedProp = node.anchor.GetComponent<ElectrocutedProp>();
            if (electrocutedProp && electrocutedProp.activated)
            {
                if (electrocutedProp.sender)
                {
                    checkedElectrocution = true;
                }
                else if (electrocuted)
                {
                    electrocutedProp.light.material.color = Color.yellow;
                }
            }
        }
        electrocuted = checkedElectrocution;
    }
}

[Serializable] public class Node
{
    public int index;
    public GameObject nodePoint;
    public bool positive;
    public GameObject anchor;
}