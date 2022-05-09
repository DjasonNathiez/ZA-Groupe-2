using System;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
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
    public PlayerManager playerManager;
    public GameObject pinnedTo;
    public int ropeHealth;
    public float lenghtToStick;
    public bool clamped;

    void Update()
    {
        if (ropeHealth <= 0)
        {
            ResetPin();
        }

        //CHECK LES TRUCS TOUCH POUR LES FAIRES TOMBER
        CheckToFall();
        
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
            Debug.Log("hitté ");
            if (!hit.collider.isTrigger)
            {
                Debug.Log("pasTrigger ");
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
        }
        Debug.DrawRay(ray.origin + Vector3.up,ray.direction * Vector3.Distance(transform.position, point),Color.red);

        // CHECK DE NOUVELLES COLLISIONS AVEC LE PREMIER SEGMENT
        
        if (nodes.Count > 0)
        {
            dir = pin.transform.position - (nodes[0].nodePoint.transform.position);
            ray = new Ray((nodes[0].nodePoint.transform.position)+dir.normalized*checkDistance, dir);
            if (Physics.Raycast(ray, out hit, dir.magnitude-checkDistance))
            {
                if (!hit.collider.isTrigger)
                {
                    
                    Vector3 pos = hit.collider.ClosestPoint(hit.point) +
                                  (hit.point - hit.transform.position).normalized * borderDist;
                    if (Vector3.Distance(pos, pin.transform.position) > checkDistance &&
                        Vector3.Distance(pos, ray.origin) > checkDistance)
                    {
                        rope.positionCount += 1;
                        Node nodeToCreate = new Node();
                        nodeToCreate.index = 1;
                        nodeToCreate.anchor = hit.transform.gameObject;
                        nodeToCreate.nodePoint = Instantiate(nodePos, new Vector3(pos.x, transform.position.y, pos.z),
                            Quaternion.identity, nodeToCreate.anchor.transform);
                        nodeToCreate.nodePoint.name = "Node " + nodeToCreate.index;
                        float signedAngle = Vector3.SignedAngle(
                            nodeToCreate.anchor.transform.position + rope.transform.position -
                            (nodes[0].nodePoint.transform.position + rope.transform.position),
                            dir, Vector3.up);
                        if (signedAngle > 0)
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
                            rope.SetPosition(nodeToFix.index, nodeToFix.nodePoint.transform.position);
                        }

                        nodes.Insert(0, nodeToCreate);
                        CheckElectrocution();
                    }
                }
            }  
            Debug.DrawRay(ray.origin + Vector3.up,ray.direction * (dir.magnitude-checkDistance),Color.blue);
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
                    if (!hitNode.collider.isTrigger)
                    {
                        Vector3 pos = hitNode.collider.ClosestPoint(hitNode.point) +
                                      (hitNode.point - hitNode.transform.position).normalized * borderDist;
                        if (Vector3.Distance(pos, node.nodePoint.transform.position) > checkDistance &&
                            Vector3.Distance(pos, nodes[node.index - 2].nodePoint.transform.position) > checkDistance)
                        {
                            rope.positionCount += 1;

                            Node nodeToCreate = new Node();
                            nodeToCreate.index = node.index;
                            nodeToCreate.anchor = hitNode.transform.gameObject;
                            nodeToCreate.nodePoint = Instantiate(nodePos,
                                new Vector3(pos.x, transform.position.y, pos.z), Quaternion.identity,
                                nodeToCreate.anchor.transform);
                            nodeToCreate.nodePoint.name = "Node " + nodeToCreate.index;
                            float signedAngle = Vector3.SignedAngle(
                                nodeToCreate.anchor.transform.position -
                                (nodes[node.index - 1].nodePoint.transform.position),
                                dirNode, Vector3.up);
                            if (signedAngle > 0)
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

                                rope.SetPosition(nodeToFix.index, nodeToFix.nodePoint.transform.position);
                            }

                            nodes.Insert(node.index - 2, nodeToCreate);
                            CheckElectrocution();
                            break;
                        }
                    }
                }
                Debug.DrawRay(rayNode.origin+Vector3.up,rayNode.direction*Vector3.Distance(nodes[node.index - 2].nodePoint.transform.position, node.nodePoint.transform.position),Color.green);
            }
        }
        rope.SetPosition(rope.positionCount-1,transform.position-rope.transform.position);
        rope.SetPosition(0,pin.transform.position-rope.transform.position);
        foreach (Node node in nodes)
        {
            rope.SetPosition(node.index,node.nodePoint.transform.position-rope.transform.position);
        }

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
                        if (nodes[i - 3].anchor.GetComponent<ElectrocutedProp>())
                        {
                            nodes[i-3].anchor.GetComponent<ElectrocutedProp>().LightsOff();   
                            
                            if (nodes[i - 3].anchor.GetComponent<ElectrocutedProp>().isEyePillar)
                            {
                                nodes[i-3].anchor.GetComponent<ElectrocutedProp>().RemoveToEyePillar();
                            }
                        }
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
                        if (nodes[i - 3].anchor.GetComponent<ElectrocutedProp>())
                        {
                            nodes[i-3].anchor.GetComponent<ElectrocutedProp>().LightsOff();

                            if (nodes[i - 3].anchor.GetComponent<ElectrocutedProp>().isEyePillar)
                            {
                                nodes[i-3].anchor.GetComponent<ElectrocutedProp>().RemoveToEyePillar();
                            }
                        }
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

        // AUTRES TRUCS DE PHYSIQUE DE TIRAGE D'OBJET

        float checklenght = 0;
        for (int i = 1; i < rope.positionCount; i++)
        {
            checklenght += Vector3.Distance(rope.GetPosition(i), rope.GetPosition(i - 1));
        }
        
        remainingLenght = maximumLenght - (checklenght - Vector3.Distance(rope.GetPosition(rope.positionCount-2), rope.GetPosition(rope.positionCount-1)));;

        lenght = checklenght;
        
        
        if (pinnedToObject)
        {
            if (clamped && lenght > 1.5f)
            {
                Vector3 force = (rope.GetPosition(1) - rope.GetPosition(0)).normalized;
                pinnedRb.AddForceAtPosition(force * 30 ,pin.transform.position,ForceMode.Acceleration);
            }
        }
        
        if (Vector3.Distance(rope.GetPosition(rope.positionCount-2), rope.GetPosition(rope.positionCount-1)) > remainingLenght)
        {

            Vector3 newPos = (rope.GetPosition(rope.positionCount - 2)+rope.transform.position) + (transform.position - (rope.GetPosition(rope.positionCount - 2)+rope.transform.position)).normalized * remainingLenght;
            transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
            
        }

        // REWINDING DU CABLE
        
        if (rewinding)
        {
            if (pinnedTo)
            {
                if (pinnedTo.GetComponent<ElectrocutedProp>() && !pinnedTo.GetComponent<ElectrocutedProp>().sender)
                {
                    pinnedTo.GetComponent<ElectrocutedProp>().LightsOff();
                    
                    if (pinnedTo.GetComponent<ElectrocutedProp>().isEyePillar)
                    {
                        pinnedTo.GetComponent<ElectrocutedProp>().RemoveToEyePillar();
                    }
                }
                pinnedTo = null;
            }
            electrocuted = false;
            if (nodes.Count > 0)
            {
                float newlenght = 50;
                int check = nodes.Count;
                for (int i = 0; i < check; i++)
                {
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
                            pin.transform.position = nodes[0].nodePoint.transform.position;
                            for (int N = 1; N < rope.positionCount-1; N++)
                            {
                                rope.SetPosition(N,rope.GetPosition(N+1));
                            }
                            Destroy(nodes[0].nodePoint);
                            if (nodes[0].anchor.GetComponent<ElectrocutedProp>())
                            {
                                nodes[0].anchor.GetComponent<ElectrocutedProp>().LightsOff();   
                                
                                if (nodes[0].anchor.GetComponent<ElectrocutedProp>().isEyePillar)
                                {
                                    nodes[0].anchor.GetComponent<ElectrocutedProp>().RemoveToEyePillar();
                                }
                            }
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
                    playerManager.state = "StatusQuo";
                    rope.gameObject.SetActive(false);
                    pinnedToObject = false;
                    rewinding = false;
                    enabled = false;
                }
            }
        }
    }

    public void OnClamp()
    {
        clamped = true;
    }
    
    public void OnUnclamp()
    {
        clamped = false;
    }

    public Vector3[] CalculateCuttingPoints(float resolution)
    {
        List<Vector3> positions = new List<Vector3>();
        
        Vector3 point;
        if (nodes.Count > 0)
        {
            point = nodes[nodes.Count - 1].nodePoint.transform.position;
            
            float dist = Vector3.Distance(point, transform.position);
            int numberOfPoints = Mathf.RoundToInt( Mathf.FloorToInt(dist) * resolution );
            for (int i = 1; i < numberOfPoints; i++)
            {
                Vector3 pos = transform.position +
                              (point - transform.position).normalized * ((dist / numberOfPoints) * i);
                positions.Add(pos);
            }
            
            point = nodes[0].nodePoint.transform.position;
            
             dist = Vector3.Distance(point, pin.transform.position);
             numberOfPoints = Mathf.RoundToInt( Mathf.FloorToInt(dist) * resolution );
            for (int i = 1; i < numberOfPoints; i++)
            {
                Vector3 pos = pin.transform.position +
                              (point - pin.transform.position).normalized * ((dist / numberOfPoints) * i);
                positions.Add(pos);
            }
        }
        else
        {
            point = pin.transform.position;
            float dist = Vector3.Distance(point, transform.position);
            int numberOfPoints = Mathf.RoundToInt( Mathf.FloorToInt(dist) * resolution );
            for (int i = 1; i < numberOfPoints; i++)
            {
                Vector3 pos = transform.position +
                              (point - transform.position).normalized * ((dist / numberOfPoints) * i);
                positions.Add(pos);
            }
        }
        
        
        
        foreach (Node node in nodes)
        {
            if (node.index > 1 && nodes[node.index - 2].anchor != node.anchor)
            {
                float dist = Vector3.Distance(node.nodePoint.transform.position, nodes[node.index - 2].nodePoint.transform.position);
                int numberOfPoints = Mathf.RoundToInt( Mathf.FloorToInt(dist) * resolution );
                for (int i = 1; i < numberOfPoints; i++)
                {
                    Vector3 pos = node.nodePoint.transform.position +
                                  (nodes[node.index - 2].nodePoint.transform.position - node.nodePoint.transform.position).normalized * ((dist / numberOfPoints) * i);
                    positions.Add(pos);
                }
            }
        }

        return positions.ToArray();
    }

    public void ResetPin()
    {
        pin.SetActive(false);
        playerManager.state = "StatusQuo";
        pinnedToObject = false;
        rewinding = false;
        enabled = false;
    }
    public void CheckElectrocution()
    {
        bool checkedElectrocution = false;
        if (pinnedTo)
        {
            ElectrocutedProp electrocutedPropPin = pinnedTo.GetComponent<ElectrocutedProp>();
            
            if (electrocutedPropPin && electrocutedPropPin.activated && electrocutedPropPin.sender)
            {
                checkedElectrocution = true;
            }

            if (electrocuted && electrocutedPropPin && electrocutedPropPin.activated && !electrocutedPropPin.sender)
            {
                electrocutedPropPin.LightsOn();
            }
        }
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
                    electrocutedProp.LightsOn();
                }
            }

            if (electrocutedProp && electrocutedProp.isEyePillar)
            {
                electrocutedProp.AddToEyePillar();
            }
        }
        electrocuted = checkedElectrocution;
    }

    public void CheckToFall()
    {
        foreach (Node node in nodes)
        {
            AIBrain ai = node.anchor.GetComponent<AIBrain>();
            
            if (ai && ai.canFall)
            {
                ai.isFalling = true;
                rewinding = true;
            }
            
        }
    }
}

[Serializable] public class Node
{
    public int index;
    public GameObject nodePoint;
    public bool positive;
    public GameObject anchor;
}