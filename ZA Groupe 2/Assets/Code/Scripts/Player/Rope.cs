using System;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public GameObject pin;
    public bool pinnedToObject;
    public Rigidbody pinnedRb;
    public float gripY;
    public float yCheckDistance;
    public LineRenderer rope;
    public Material unpowered;
    public Material powered;
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
    public ValueTrack pinnedValueTrack;
    public int ropeHealth;
    public float lenghtToStick;
    public bool clamped;
    public bool rightTrig;
    public float stickLenght;
    public bool leftTrig;
    public AnimationCurve reactorStrenght;
    public float memoryTemp;

    void Update()
    {
        if (!rewinding)
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
                if (!hit.collider.isTrigger)
                {
                    rope.positionCount += 1;
                    Node nodeToCreate = new Node();
                    nodeToCreate.index = rope.positionCount - 2;
                    nodeToCreate.anchor = hit.transform.gameObject;
                    Vector3 pos = hit.collider.ClosestPoint(hit.point) +
                                  (hit.point - hit.transform.position).normalized * borderDist;
                    nodeToCreate.nodePoint = Instantiate(nodePos, new Vector3(pos.x, transform.position.y, pos.z),
                        Quaternion.identity, nodeToCreate.anchor.transform);
                    nodeToCreate.nodePoint.name = "Node " + nodeToCreate.index;

                    if (Vector3.SignedAngle(nodeToCreate.anchor.transform.position - transform.position, dir,
                        Vector3.up) > 0)
                    {
                        nodeToCreate.positive = true;
                    }
                    else
                    {
                        nodeToCreate.positive = false;
                    }

                    nodes.Add(nodeToCreate);

                    if (rightTrig || leftTrig) rewinding = true;
                }
            }

            Debug.DrawRay(ray.origin + Vector3.up, ray.direction * Vector3.Distance(transform.position, point),
                Color.red);

            // CHECK DE NOUVELLES COLLISIONS AVEC LE PREMIER SEGMENT

            if (nodes.Count > 0)
            {
                dir = pin.transform.position - (nodes[0].nodePoint.transform.position);
                ray = new Ray((nodes[0].nodePoint.transform.position) + dir.normalized * checkDistance, dir);
                if (Physics.Raycast(ray, out hit, dir.magnitude - checkDistance * 2))
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
                            nodeToCreate.nodePoint = Instantiate(nodePos,
                                new Vector3(pos.x, transform.position.y, pos.z),
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
                            if (rightTrig || leftTrig) rewinding = true;
                        }
                    }
                }

                Debug.DrawRay(ray.origin + Vector3.up, ray.direction * (dir.magnitude - checkDistance), Color.blue);
            }

            // CHECK DE COLLISIONS AVEC TOUS LES AUTRES NODES

            foreach (Node node in nodes)
            {
                if (node.index > 1 && nodes[node.index - 2].anchor != node.anchor)
                {
                    Vector3 dirNode = nodes[node.index - 2].nodePoint.transform.position -
                                      node.nodePoint.transform.position;
                    Ray rayNode = new Ray((node.nodePoint.transform.position) + dirNode.normalized * checkDistance,
                        dirNode);
                    if (Physics.Raycast(rayNode, out RaycastHit hitNode,
                        Vector3.Distance(nodes[node.index - 2].nodePoint.transform.position,
                            node.nodePoint.transform.position)))
                    {
                        if (!hitNode.collider.isTrigger)
                        {
                            Vector3 pos = hitNode.collider.ClosestPoint(hitNode.point) +
                                          (hitNode.point - hitNode.transform.position).normalized * borderDist;
                            if (Vector3.Distance(pos, node.nodePoint.transform.position) > checkDistance &&
                                Vector3.Distance(pos, nodes[node.index - 2].nodePoint.transform.position) >
                                checkDistance)
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
                                if (rightTrig || leftTrig) rewinding = true;
                                break;
                            }
                        }
                    }

                    Debug.DrawRay(rayNode.origin + Vector3.up,
                        rayNode.direction * Vector3.Distance(nodes[node.index - 2].nodePoint.transform.position,
                            node.nodePoint.transform.position), Color.green);
                }
            }
            
            // Gotgot

            if (lenght > maximumLenght)
            {
                playerManager.Rewind();
            }
            
            
        }

        rope.SetPosition(rope.positionCount - 1, transform.position - rope.transform.position);
        rope.SetPosition(0, pin.transform.position - rope.transform.position);
        foreach (Node node in nodes)
        {
            rope.SetPosition(node.index, node.nodePoint.transform.position - rope.transform.position);
        }

        // SUPPRESSION DES NODES

        if (nodes.Count > 0)
        {
            int index = rope.positionCount;
            for (int i = index; i > 2; i--)
            {
                float angleDiffNode =
                    Vector3.SignedAngle((rope.GetPosition(i - 1) - rope.GetPosition(i - 2)).normalized,
                        (rope.GetPosition(i - 2) - rope.GetPosition(i - 3)).normalized, Vector3.up);

                if (nodes[i - 3].positive)
                {
                    if (angleDiffNode > 0)
                    {
                        for (int N = i - 2; N < rope.positionCount - 1; N++)
                        {
                            rope.SetPosition(N, rope.GetPosition(N + 1));
                        }

                        Destroy(nodes[i - 3].nodePoint);
                        if (nodes[i - 3].anchor.GetComponent<ElectrocutedProp>())
                        {
                            nodes[i - 3].anchor.GetComponent<ElectrocutedProp>().LightsOff();

                            if (nodes[i - 3].anchor.GetComponent<ElectrocutedProp>().isEyePillar)
                            {
                                nodes[i - 3].anchor.GetComponent<ElectrocutedProp>().RemoveToEyePillar();
                            }
                        }

                        nodes.RemoveAt(i - 3);
                        foreach (Node node in nodes)
                        {
                            if (node.index > i - 2)
                            {
                                node.index -= 1;
                            }
                        }

                        rope.positionCount -= 1;
                        rope.SetPosition(rope.positionCount - 1, transform.position - rope.transform.position);
                    }
                }
                else
                {
                    if (angleDiffNode < 0)
                    {
                        for (int N = i - 2; N < rope.positionCount - 1; N++)
                        {
                            rope.SetPosition(N, rope.GetPosition(N + 1));
                        }

                        Destroy(nodes[i - 3].nodePoint);
                        if (nodes[i - 3].anchor.GetComponent<ElectrocutedProp>())
                        {
                            nodes[i - 3].anchor.GetComponent<ElectrocutedProp>().LightsOff();

                            if (nodes[i - 3].anchor.GetComponent<ElectrocutedProp>().isEyePillar)
                            {
                                nodes[i - 3].anchor.GetComponent<ElectrocutedProp>().RemoveToEyePillar();
                            }
                        }

                        nodes.RemoveAt(i - 3);
                        foreach (Node node in nodes)
                        {
                            if (node.index > i - 2)
                            {
                                node.index -= 1;
                            }
                        }

                        rope.positionCount -= 1;
                        rope.SetPosition(rope.positionCount - 1, transform.position - rope.transform.position);
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

        if (clamped)
        {
            remainingLenght = lenghtToStick - (checklenght - Vector3.Distance(rope.GetPosition(rope.positionCount - 2),
                rope.GetPosition(rope.positionCount - 1)));
            ;
        }
        else
        {
            remainingLenght = maximumLenght - (checklenght - Vector3.Distance(rope.GetPosition(rope.positionCount - 2),
                rope.GetPosition(rope.positionCount - 1)));
        }

        CheckElectrocution();

        lenght = checklenght;

        if (Vector3.SqrMagnitude(rope.GetPosition(rope.positionCount - 2) - rope.GetPosition(rope.positionCount - 1)) >
            remainingLenght * remainingLenght &&
            PlayerManager.instance.state == ActionType.RopeAttached)
        {
            Vector3 newPos = (rope.GetPosition(rope.positionCount - 2) + rope.transform.position) +
                             (transform.position - (rope.GetPosition(rope.positionCount - 2) + rope.transform.position))
                             .normalized * remainingLenght;
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
                pinnedValueTrack = null;
            }

            electrocuted = false;
            clamped = false;
            if (nodes.Count > 0)
            {
                float newlenght = 50;
                int check = nodes.Count;
                for (int i = 0; i < check; i++)
                {
                    if (((nodes[0].nodePoint.transform.position - pin.transform.position).normalized * Time.deltaTime *
                         newlenght).magnitude <
                        (nodes[0].nodePoint.transform.position - pin.transform.position).magnitude &&
                        (nodes[0].nodePoint.transform.position - pin.transform.position).magnitude > 0.001f)
                    {
                        Vector3 newPos = pin.transform.position +
                                         (nodes[0].nodePoint.transform.position - pin.transform.position).normalized *
                                         Time.deltaTime * newlenght;
                        pin.transform.position = newPos;
                        break;
                    }
                    else
                    {
                        newlenght =
                            ((nodes[0].nodePoint.transform.position - pin.transform.position).normalized *
                             Time.deltaTime * newlenght).magnitude -
                            (nodes[0].nodePoint.transform.position - pin.transform.position).magnitude;
                        pin.transform.position = nodes[0].nodePoint.transform.position;
                        for (int N = 1; N < rope.positionCount - 1; N++)
                        {
                            rope.SetPosition(N, rope.GetPosition(N + 1));
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
                        rope.SetPosition(rope.positionCount - 1, transform.position - rope.transform.position);
                    }
                }
            }
            else
            {
                if (((transform.position - pin.transform.position).normalized * Time.deltaTime * 50).magnitude <
                    (transform.position - pin.transform.position).magnitude)
                {
                    Vector3 newPos = pin.transform.position +
                                     (transform.position - pin.transform.position).normalized * Time.deltaTime * 50;
                    pin.transform.position = newPos;
                }
                else
                {
                    pin.SetActive(false);
                    playerManager.state = ActionType.StatusQuo;
                    playerManager.playerThrowingWeapon.SetActive(true);
                    rope.gameObject.SetActive(false);
                    pinnedToObject = false;
                    rewinding = false;
                    enabled = false;
                }
            }
        }
        else
        {
            gripY = pin.transform.position.y;
            if (transform.position.y > gripY + yCheckDistance || transform.position.y < gripY - yCheckDistance)
            {
                rewinding = true;
                //Debug.Log(transform.position.y + " and " + gripY);
            }
        }



        if (rightTrig)
        {
            float usedLenght = 0;
            if (nodes.Count > 0)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (i < nodes.Count - 1)
                    {
                        usedLenght += (nodes[i].nodePoint.transform.position - nodes[i + 1].nodePoint.transform.position).magnitude;
                    }
                    else
                    {
                        usedLenght += (nodes[i].nodePoint.transform.position - playerManager.transform.position).magnitude;
                    }
                    float remain = stickLenght - usedLenght;

                    if ((pinnedTo.transform.position - nodes[0].nodePoint.transform.position).sqrMagnitude > remain * remain)
                    {
                        pinnedTo.transform.position =
                            new Vector3(nodes[0].nodePoint.transform.position.x, pinnedTo.transform.position.y,
                                nodes[0].nodePoint.transform.position.z) + Vector3.ClampMagnitude(
                                pinnedTo.transform.position - new Vector3(nodes[0].nodePoint.transform.position.x,
                                    pinnedTo.transform.position.y, nodes[0].nodePoint.transform.position.z), remain);
                    }
                }
            }
            else
            {
                float remain = stickLenght;
                if ((pinnedTo.transform.position - playerManager.transform.position).sqrMagnitude > remain * remain)
                {
                    pinnedTo.transform.position =
                        new Vector3(playerManager.transform.position.x, pinnedTo.transform.position.y,
                            playerManager.transform.position.z) + Vector3.ClampMagnitude(
                            pinnedTo.transform.position - new Vector3(playerManager.transform.position.x,
                                pinnedTo.transform.position.y, playerManager.transform.position.z), remain);
                }
            }
        }
        
        if (rightTrig || leftTrig)
        {
            float usedLenght = 0;
            if (nodes.Count > 0)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (i < nodes.Count - 1)
                    {
                        usedLenght += (nodes[i].nodePoint.transform.position - nodes[i + 1].nodePoint.transform.position).magnitude;
                    }
                    else
                    {
                        usedLenght += (nodes[i].nodePoint.transform.position - playerManager.transform.position).magnitude;
                    }
                    float remain = stickLenght - usedLenght;

                    if ((pinnedTo.transform.position - nodes[0].nodePoint.transform.position).sqrMagnitude > remain * remain)
                    {
                        pinnedTo.transform.position =
                            new Vector3(nodes[0].nodePoint.transform.position.x, pinnedTo.transform.position.y,
                                nodes[0].nodePoint.transform.position.z) + Vector3.ClampMagnitude(
                                pinnedTo.transform.position - new Vector3(nodes[0].nodePoint.transform.position.x,
                                    pinnedTo.transform.position.y, nodes[0].nodePoint.transform.position.z), remain);
                    }
                }
            }
            else
            {
                float remain = stickLenght;
                if ((pinnedTo.transform.position - playerManager.transform.position).sqrMagnitude > remain * remain)
                {
                    pinnedTo.transform.position =
                        new Vector3(playerManager.transform.position.x, pinnedTo.transform.position.y,
                            playerManager.transform.position.z) + Vector3.ClampMagnitude(
                            pinnedTo.transform.position - new Vector3(playerManager.transform.position.x,
                                pinnedTo.transform.position.y, playerManager.transform.position.z), remain);
                }
            }
        }
    }

    public void FindStickLenght()
    {
        float newStick = 0;
        if (nodes.Count > 0)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (i < nodes.Count - 1)
                {
                    newStick += (nodes[i].nodePoint.transform.position - nodes[i + 1].nodePoint.transform.position).magnitude;
                }
                else
                {
                    newStick += (nodes[i].nodePoint.transform.position - playerManager.transform.position).magnitude;
                }
            }

            newStick += (nodes[0].nodePoint.transform.position - pinnedTo.transform.position).magnitude;
        }
        else
        {
            newStick = (playerManager.transform.position - pinnedTo.transform.position).magnitude;
        }

        stickLenght = newStick;
    }

    private void FixedUpdate()
    {
        if (pinnedToObject)
        {
            if (clamped && lenght > 1.5f)
            {
                Vector3 force = (rope.GetPosition(1) - rope.GetPosition(0)).normalized;
                float factor = new float();
                WeightClass objectToPull = WeightClass.LIGHT;
                objectToPull = pinnedRb.gameObject.GetComponent<ValueTrack>() != null
                    ? pinnedRb.gameObject.GetComponent<ValueTrack>().weightClass
                    : WeightClass.NULL;

                factor = objectToPull switch
                {
                    WeightClass.NULL => 0,
                    WeightClass.LIGHT => 20,
                    WeightClass.MEDIUM => 10,
                    WeightClass.HEAVY => 5,
                    _ => factor
                };

                pinnedRb.AddForceAtPosition(force * factor, pin.transform.position, ForceMode.Acceleration);
                pinnedRb.velocity = Vector3.ClampMagnitude(pinnedRb.velocity, 5);
            }
            
            if ((rightTrig && !leftTrig) || (!rightTrig && leftTrig))
            {
                Vector3 rotationCenter = playerManager.transform.position;

                if (nodes.Count > 0)
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        if (nodes[i].anchor != pinnedTo)
                        {
                            rotationCenter = nodes[i].nodePoint.transform.position;
                            break;
                        }
                    }   
                }

                Vector2 rotationVector = (new Vector2(pinnedTo.transform.position.x, pinnedTo.transform.position.z) - new Vector2(rotationCenter.x, rotationCenter.z)).normalized;

                Vector3 forceToUse;
                
                if(rightTrig) forceToUse = new Vector3(rotationVector.y, 0, -rotationVector.x);
                else forceToUse = new Vector3(-rotationVector.y, 0, rotationVector.x);

                Debug.DrawRay(pinnedTo.transform.position,new Vector3(rotationVector.x,0,rotationVector.y),Color.green);

                float factor = new float();
                WeightClass objectToPull = WeightClass.LIGHT;
                objectToPull = pinnedRb.gameObject.GetComponent<ValueTrack>() != null
                    ? pinnedRb.gameObject.GetComponent<ValueTrack>().weightClass
                    : WeightClass.NULL;

                factor = objectToPull switch
                {
                    WeightClass.NULL => 0,
                    WeightClass.LIGHT => 20,
                    WeightClass.MEDIUM => 10,
                    WeightClass.HEAVY => 5,
                    _ => factor
                };

                pinnedRb.AddForce(forceToUse*reactorStrenght.Evaluate(Time.time-memoryTemp), ForceMode.VelocityChange);
                pinnedRb.velocity = Vector3.ClampMagnitude(pinnedRb.velocity, 10);
                
                // TROUVER ANGLE ROTATION

               Vector3 dirGrip = pin.transform.position - pinnedTo.transform.position;
                Vector3 dirToAlign = nodes.Count > 0
                    ? (nodes[0].nodePoint.transform.position - pinnedTo.transform.position)
                    : (playerManager.transform.position - pinnedTo.transform.position);

                float angle = Vector3.SignedAngle(dirGrip, dirToAlign, Vector3.up);

                Quaternion alignement = Quaternion.Euler(pinnedTo.transform.eulerAngles.x,pinnedTo.transform.eulerAngles.y + angle,pinnedTo.transform.eulerAngles.z);
                
                pinnedRb.AddTorque(0,angle,0);
            }
        }
    }

    public void OnClamp()
    {
        if (PlayerManager.instance.state == ActionType.RopeAttached)
        {
            clamped = true;
            lenghtToStick = lenght;
        }
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
            int numberOfPoints = Mathf.RoundToInt(Mathf.FloorToInt(dist) * resolution);
            for (int i = 1; i < numberOfPoints; i++)
            {
                Vector3 pos = transform.position +
                              (point - transform.position).normalized * ((dist / numberOfPoints) * i);
                positions.Add(pos);
            }

            point = nodes[0].nodePoint.transform.position;

            dist = Vector3.Distance(point, pin.transform.position);
            numberOfPoints = Mathf.RoundToInt(Mathf.FloorToInt(dist) * resolution);
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
            int numberOfPoints = Mathf.RoundToInt(Mathf.FloorToInt(dist) * resolution);
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
                float dist = Vector3.Distance(node.nodePoint.transform.position,
                    nodes[node.index - 2].nodePoint.transform.position);
                int numberOfPoints = Mathf.RoundToInt(Mathf.FloorToInt(dist) * resolution);
                for (int i = 1; i < numberOfPoints; i++)
                {
                    Vector3 pos = node.nodePoint.transform.position +
                                  (nodes[node.index - 2].nodePoint.transform.position -
                                   node.nodePoint.transform.position).normalized * ((dist / numberOfPoints) * i);
                    positions.Add(pos);
                }
            }
        }

        return positions.ToArray();
    }

    public void ResetPin()
    {
        pin.SetActive(false);
        playerManager.state = ActionType.StatusQuo;
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
            else if (!electrocuted && electrocutedPropPin && electrocutedPropPin.activated &&
                     !electrocutedPropPin.sender)
            {
                electrocutedPropPin.LightsOff();
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
                else if (!electrocuted)
                {
                    electrocutedProp.LightsOff();
                }
            }

            if (electrocutedProp && electrocutedProp.isEyePillar)
            {
                electrocutedProp.AddToEyePillar();
            }
        }

        electrocuted = checkedElectrocution;
        if (electrocuted) rope.material = powered;
        else rope.material = unpowered;
    }

    public void CheckToFall()
    {
        foreach (Node node in nodes)
        {
            BearBehaviour ai = node.anchor.GetComponentInParent<BearBehaviour>();

            if (ai && ai.canFall && GetComponent<PlayerManager>().state == ActionType.RopeAttached)
            {
                ai.FallOnTheGround();
                rewinding = true;
                //Debug.Log("ReasonNumberThree");
            }
        }
    }
}

[Serializable]
public class Node
{
    public int index;
    public GameObject nodePoint;
    public bool positive;
    public GameObject anchor;
}