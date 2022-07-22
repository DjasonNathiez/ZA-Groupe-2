using System;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    #region Variables

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


    private float holdTime;
    private bool isShakeActive;
    [SerializeField] private CameraShakeScriptable holdCrateShake;

    #endregion


    void Update()
    {
        if (!rewinding)
        {
            CheckDestroyed();

            if (ropeHealth <= 0)
            {
                ResetPin();
            }

            AttributeDirection();

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
                if (!hit.collider.isTrigger || hit.collider.CompareTag("Bullet"))
                {
                    if (hit.collider.CompareTag("Bullet") && !hit.transform.GetComponent<bulletBehavior>().canBounce)
                        goto exitOne;
                    bulletBehavior bulletBehavior = hit.transform.GetComponent<bulletBehavior>();


                    rope.positionCount += 1;
                    Node nodeToCreate = new Node();
                    nodeToCreate.index = rope.positionCount - 2;
                    nodeToCreate.anchor = hit.transform.gameObject;
                    Vector3 pos = hit.collider.ClosestPoint(hit.point) +
                                  (hit.point - hit.transform.position).normalized * borderDist;
                    nodeToCreate.nodePoint = Instantiate(nodePos, new Vector3(pos.x, transform.position.y, pos.z),
                        Quaternion.identity, nodeToCreate.anchor.transform);

                    if (hit.collider.CompareTag("Bullet") && bulletBehavior.canBounce)
                    {
                        nodeToCreate.nodePoint.transform.position =
                            hit.transform.position + bulletBehavior.velocity.normalized * 0.25f;
                    }

                    nodeToCreate.nodePoint.name = "Node " + nodeToCreate.index;
                    nodeToCreate.former = transform;
                    nodeToCreate.later =
                        (nodes.Count == 0 ? pin.transform : nodes[nodes.Count - 1].nodePoint.transform);
                    Vector3 former = transform.position - nodeToCreate.nodePoint.transform.position;
                    Vector3 later = nodeToCreate.nodePoint.transform.position - (nodes.Count == 0
                        ? pin.transform.position
                        : nodes[nodes.Count - 1].nodePoint.transform.position);

                    nodeToCreate.angleBuffer = Vector3.SignedAngle(former, later, Vector3.up);

                    if (hit.collider.CompareTag("Bullet") && bulletBehavior.canBounce)
                    {
                        Vector3 inNormal = Quaternion.AngleAxis(90, Vector3.up) *
                                           new Vector3(ray.direction.x, 0, ray.direction.z).normalized *
                                           (nodeToCreate.angleBuffer > 0 ? -1 : 1);
                        if (Vector3.Angle(bulletBehavior.velocity, inNormal) > 90) goto exitOne;
                        bulletBehavior.Bounce(inNormal);
                    }

                    nodeToCreate.direction = NodeDirection.UNDEFINED;

                    nodes.Add(nodeToCreate);
                }
            }

            exitOne:

            Debug.DrawRay(ray.origin + Vector3.up, ray.direction * Vector3.Distance(transform.position, point),
                Color.red);

            // CHECK DE NOUVELLES COLLISIONS AVEC LE PREMIER SEGMENT

            if (nodes.Count > 0)
            {
                dir = pin.transform.position - (nodes[0].nodePoint.transform.position);
                ray = new Ray((nodes[0].nodePoint.transform.position) + dir.normalized * checkDistance, dir);
                if (Physics.Raycast(ray, out hit, dir.magnitude - checkDistance * 2))
                {
                    if (!hit.collider.isTrigger || hit.collider.CompareTag("Bullet"))
                    {
                        if (hit.collider.CompareTag("Bullet") &&
                            !hit.transform.GetComponent<bulletBehavior>().canBounce) goto exitTwo;
                        bulletBehavior bulletBehavior = hit.transform.GetComponent<bulletBehavior>();

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

                            if (hit.collider.CompareTag("Bullet") && bulletBehavior.canBounce)
                            {
                                nodeToCreate.nodePoint.transform.position = hit.transform.position +
                                                                            bulletBehavior.velocity.normalized * 0.25f;
                            }

                            nodeToCreate.nodePoint.name = "Node " + nodeToCreate.index;
                            nodeToCreate.former = nodes[0].nodePoint.transform;
                            nodeToCreate.later = pin.transform;
                            Vector3 former = nodes[0].nodePoint.transform.position -
                                             nodeToCreate.nodePoint.transform.position;
                            Vector3 later = nodeToCreate.nodePoint.transform.position - pin.transform.position;
                            nodeToCreate.angleBuffer = Vector3.SignedAngle(former, later, Vector3.up);

                            if (hit.collider.CompareTag("Bullet") && bulletBehavior.canBounce)
                            {
                                Vector3 inNormal = Quaternion.AngleAxis(90, Vector3.up) *
                                                   new Vector3(ray.direction.x, 0, ray.direction.z).normalized *
                                                   (nodeToCreate.angleBuffer > 0 ? -1 : 1);
                                if (Vector3.Angle(bulletBehavior.velocity, inNormal) > 90) goto exitTwo;
                                bulletBehavior.Bounce(inNormal);
                            }

                            nodeToCreate.direction = NodeDirection.UNDEFINED;

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

                exitTwo:

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
                        if (!hitNode.collider.isTrigger || hitNode.collider.CompareTag("Bullet"))
                        {
                            if (hitNode.collider.CompareTag("Bullet") &&
                                !hitNode.transform.GetComponent<bulletBehavior>().canBounce) goto exitThree;
                            bulletBehavior bulletBehavior = hitNode.transform.GetComponent<bulletBehavior>();

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

                                if (hitNode.collider.CompareTag("Bullet") && bulletBehavior.canBounce)
                                {
                                    nodeToCreate.nodePoint.transform.position = hitNode.transform.position +
                                        bulletBehavior.velocity.normalized * 0.25f;
                                }

                                nodeToCreate.nodePoint.name = "Node " + nodeToCreate.index;
                                nodeToCreate.former = node.nodePoint.transform;
                                nodeToCreate.later = nodes[node.index - 2].nodePoint.transform;
                                Vector3 former = node.nodePoint.transform.position -
                                                 nodeToCreate.nodePoint.transform.position;
                                Vector3 later = nodeToCreate.nodePoint.transform.position -
                                                nodes[node.index - 2].nodePoint.transform.position;
                                nodeToCreate.angleBuffer = Vector3.SignedAngle(former, later, Vector3.up);

                                if (hitNode.collider.CompareTag("Bullet") && bulletBehavior.canBounce)
                                {
                                    Vector3 inNormal = Quaternion.AngleAxis(90, Vector3.up) *
                                                       new Vector3(ray.direction.x, 0, ray.direction.z).normalized *
                                                       (nodeToCreate.angleBuffer > 0 ? -1 : 1);
                                    if (Vector3.Angle(bulletBehavior.velocity, inNormal) > 90) goto exitThree;
                                    bulletBehavior.Bounce(inNormal);
                                }

                                nodeToCreate.direction = NodeDirection.UNDEFINED;

                                foreach (Node nodeToFix in nodes)
                                {
                                    if (nodeToFix.index >= node.index)
                                    {
                                        nodeToFix.index += 1;
                                    }
                                }


                                nodes.Insert(node.index - 2, nodeToCreate);

                                break;
                            }
                        }
                    }

                    exitThree:

                    Debug.DrawRay(rayNode.origin + Vector3.up,
                        rayNode.direction * Vector3.Distance(nodes[node.index - 2].nodePoint.transform.position,
                            node.nodePoint.transform.position), Color.green);
                }
                /*else if (node.index > 1)
                {
                    Vector3 dirNode = nodes[node.index - 2].nodePoint.transform.position -
                                      node.nodePoint.transform.position;
                    Ray rayNode = new Ray((node.nodePoint.transform.position) + dirNode.normalized * checkDistance,
                        dirNode);
                    if (Physics.Raycast(rayNode, out RaycastHit hitNode,
                        Vector3.Distance(nodes[node.index - 2].nodePoint.transform.position,
                            node.nodePoint.transform.position)))
                    {
                        if (hitNode.transform.gameObject == node.anchor)
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
                                nodeToCreate.former = node.nodePoint.transform;
                                nodeToCreate.later = nodes[node.index - 2].nodePoint.transform;
                                Vector3 former = node.nodePoint.transform.position - nodeToCreate.nodePoint.transform.position;
                                Vector3 later = nodeToCreate.nodePoint.transform.position - nodes[node.index - 2].nodePoint.transform.position;
                                nodeToCreate.angleBuffer = Vector3.SignedAngle(former,later, Vector3.up);

                                nodeToCreate.direction = NodeDirection.UNDEFINED;

                                foreach (Node nodeToFix in nodes)
                                {
                                    if (nodeToFix.index >= nodeToCreate.index)
                                    {
                                        nodeToFix.index += 1;
                                    }

                                    rope.SetPosition(nodeToFix.index, nodeToFix.nodePoint.transform.position);
                                }
                                
                                nodes.Insert(nodeToCreate.index - 2, nodeToCreate);
                                
                                break;
                            }
                        }
                    }
                }*/
            }
        }

        rope.positionCount = nodes.Count + 2;

        rope.SetPosition(rope.positionCount - 1, transform.position - rope.transform.position);
        rope.SetPosition(0, pin.transform.position - rope.transform.position);
        foreach (Node node in nodes)
        {
            rope.SetPosition(node.index, node.nodePoint.transform.position - rope.transform.position);
        }

        // SUPPRESSION DES NODES

        if (nodes.Count > 0)
        {
            int index = nodes.Count - 1;
            for (int i = index; i > -1; i--)
            {
                float angleDiffNode =
                    Vector3.SignedAngle(
                        (nodes[i].nodePoint.transform.position - (i < nodes.Count - 1
                            ? nodes[i + 1].nodePoint.transform.position
                            : transform.position)).normalized,
                        ((i > 0 ? nodes[i - 1].nodePoint.transform.position : pin.transform.position) -
                         nodes[i].nodePoint.transform.position).normalized, Vector3.up);

                if (nodes[i].direction == NodeDirection.POSITIVE)
                {
                    if (angleDiffNode > 0)
                    {
                        DestroyNode(i);
                    }
                }
                else if (nodes[i].direction == NodeDirection.NEGATIVE)
                {
                    if (angleDiffNode < 0)
                    {
                        DestroyNode(i);
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

                if (pinnedTo.GetComponent<ValueTrack>())
                {
                    var vt = pinnedTo.GetComponent<ValueTrack>();
                    if (vt.trailVFX)
                    {
                        vt.trailVFX.Stop();
                        vt.trailVFX.gameObject.SetActive(false);
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
    }


    public void DestroyNode(int index)
    {
        for (int N = index + 1; N < rope.positionCount - 1; N++)
        {
            rope.SetPosition(N, rope.GetPosition(N + 1));
        }

        Destroy(nodes[index].nodePoint);
        if (nodes[index].anchor && nodes[index].anchor.GetComponent<ElectrocutedProp>())
        {
            nodes[index].anchor.GetComponent<ElectrocutedProp>().LightsOff();

            if (nodes[index].anchor.GetComponent<ElectrocutedProp>().isEyePillar)
            {
                nodes[index].anchor.GetComponent<ElectrocutedProp>().RemoveToEyePillar();
            }
        }

        nodes.RemoveAt(index);
        foreach (Node node in nodes)
        {
            if (node.index > index + 1)
            {
                node.index -= 1;
            }
        }

        rope.positionCount -= 1;
        rope.SetPosition(rope.positionCount - 1, transform.position - rope.transform.position);
    }

    public void AttributeDirection()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].direction == NodeDirection.UNDEFINED)
            {
                float angle;

                if (i > 0 && nodes[i].anchor == nodes[i - 1].anchor &&
                    nodes[i - 1].direction != NodeDirection.UNDEFINED)
                {
                    nodes[i].direction = nodes[i - 1].direction;
                }
                else if (i < nodes.Count - 1 && nodes[i].anchor == nodes[i + 1].anchor)
                {
                    nodes[i].direction = nodes[i + 1].direction;
                }
                else if (nodes[i].former && nodes[i].later)
                {
                    Vector3 former = nodes[i].former.position - nodes[i].nodePoint.transform.position;
                    Vector3 later = nodes[i].nodePoint.transform.position - nodes[i].later.position;
                    angle = Vector3.SignedAngle(former, later, Vector3.up);
                    Debug.DrawRay(nodes[i].nodePoint.transform.position + Vector3.up, former, Color.green, 10);
                    Debug.DrawRay(nodes[i].later.position + Vector3.up, later, Color.magenta, 10);
                    Debug.Log(angle);

                    if (angle > nodes[i].angleBuffer)
                    {
                        nodes[i].direction = NodeDirection.NEGATIVE;
                        Debug.Log("Negative");
                    }
                    else if (angle < nodes[i].angleBuffer)
                    {
                        nodes[i].direction = NodeDirection.POSITIVE;
                        Debug.Log("Positive");
                    }
                    else
                    {
                        if (nodes[i].angleBuffer > 0) nodes[i].direction = NodeDirection.NEGATIVE;
                        else nodes[i].direction = NodeDirection.POSITIVE;
                    }
                }
                else
                {
                    if (nodes[i].angleBuffer > 0) nodes[i].direction = NodeDirection.NEGATIVE;
                    else nodes[i].direction = NodeDirection.POSITIVE;
                }
            }
        }
    }

    public void CheckDestroyed()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (!nodes[i].anchor || !nodes[i].nodePoint)
            {
                DestroyNode(i);
            }
        }
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

                holdTime += Time.deltaTime;
                if (holdTime > 1f)
                {
                    PlayerManager.instance.heavyVFX.SetActive(true);
                    if (!isShakeActive)
                    {
                        CameraShake.Instance.AddShakeEvent(holdCrateShake);
                        isShakeActive = true;
                    }
                }

                pinnedRb.AddForceAtPosition(force * factor, pin.transform.position, ForceMode.Acceleration);
                pinnedRb.velocity = Vector3.ClampMagnitude(pinnedRb.velocity, 5);
            }
            else
            {
                holdTime = 0;
                PlayerManager.instance.heavyVFX.SetActive(false);
                if (isShakeActive)
                {
                    CameraShake.Instance.shakeEvents.Clear();
                }

                isShakeActive = false;
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

    public List<Vector3> CalculateCuttingPoints(float resolution)
    {
        List<Vector3> positions = new List<Vector3>();

        Vector3 point;
        if (nodes.Count > 0)
        {
            point = nodes[nodes.Count - 1].nodePoint.transform.position;

            float dist = Vector3.Distance(point, transform.position);
            int numberOfPoints = Mathf.RoundToInt(Mathf.CeilToInt(dist) * resolution);
            for (int i = 1; i < numberOfPoints; i++)
            {
                Vector3 pos = transform.position +
                              (point - transform.position).normalized * ((dist / numberOfPoints) * i);
                positions.Add(pos);
            }

            point = nodes[0].nodePoint.transform.position;

            dist = Vector3.Distance(point, pin.transform.position);
            numberOfPoints = Mathf.RoundToInt(Mathf.CeilToInt(dist) * resolution);
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
            int numberOfPoints = Mathf.RoundToInt(Mathf.CeilToInt(dist) * resolution);
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
                int numberOfPoints = Mathf.RoundToInt(Mathf.CeilToInt(dist) * resolution);
                for (int i = 1; i < numberOfPoints; i++)
                {
                    Vector3 pos = node.nodePoint.transform.position +
                                  (nodes[node.index - 2].nodePoint.transform.position -
                                   node.nodePoint.transform.position).normalized * ((dist / numberOfPoints) * i);
                    positions.Add(pos);
                }
            }
        }

        return positions;
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

            //BUG Node Anchor Destroyed ----------------------------------------------------------------------------------------------------------------------------------------------------

            if (ai && ai.canFall && GetComponent<PlayerManager>().state == ActionType.RopeAttached)
            {
                ai.FallOnTheGround();
                rewinding = true;
            }
        }
    }
}

[Serializable]
public class Node
{
    public int index;
    public GameObject nodePoint;
    public NodeDirection direction;
    public GameObject anchor;
    public float angleBuffer;
    public Transform former;
    public Transform later;
}

public enum NodeDirection
{
    POSITIVE,
    NEGATIVE,
    UNDEFINED
}