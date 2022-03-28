using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
public class QuadraticCurve : MonoBehaviour
{
    public Anchor[] anchors;
    public float spacing = 1;
    public float resolution = 1;
    public bool looped;
    public WayPoints[] points;
    public FollowCurve followCurve;

    private void Start()
    {
        points = EvenlySpacedPoints(spacing, resolution);

        followCurve.points = points;
        followCurve.transform.position = followCurve.points[0].point;
    }


    private void OnDrawGizmos()
    {
        points = EvenlySpacedPoints(spacing, resolution);
        foreach (WayPoints pos in points)
        {
            Gizmos.DrawSphere(pos.point,0.25f);
            Gizmos.DrawRay(pos.point,pos.up*2);
        }
    }

    public Vector3 CubicCurveLerp(Vector3 pos1,Vector3 pos2,Vector3 pos3,float index)
    {
        Vector3 result = Vector3.Lerp(Vector3.Lerp(pos1, pos2, index), Vector3.Lerp(pos2, pos3, index), index);
        return result;
    }

    public Vector3 BezierCurveLerp(Vector3 anchor1, Vector3 handle1, Vector3 anchor2, Vector3 handle2, float index)
    {
        Vector3 result = Vector3.Lerp(CubicCurveLerp(anchor1, handle1,handle2, index), CubicCurveLerp(handle1, handle2, anchor2,index), index);
        return result;
    }

    public WayPoints[] EvenlySpacedPoints(float spacing, float resolution)
    {
        List<WayPoints> evenlySpacedPoints = new List<WayPoints>();
        WayPoints wayPoint = new WayPoints();
        wayPoint.point = anchors[0].anchor.position;
        wayPoint.up = anchors[0].anchor.up;
        wayPoint.speed = anchors[0].speed;
        evenlySpacedPoints.Add(wayPoint);
        Vector3 previousPoint = evenlySpacedPoints[0].point;
        float dist = 0;

        for (int a = 1; a < anchors.Length; a++)
        {
            float distanceOfOtherSegments =
                Vector3.Distance(anchors[a - 1].anchor.position, anchors[a - 1].handles[1].position) +
                Vector3.Distance(anchors[a].handles[0].position, anchors[a - 1].handles[1].position) +
                Vector3.Distance(anchors[a].handles[0].position, anchors[a].anchor.position);
            float calculatedDist = Vector3.Distance(anchors[a].anchor.position, anchors[a - 1].anchor.position) + distanceOfOtherSegments/2f;
            int divisions = Mathf.CeilToInt(calculatedDist * resolution * 10);
            float t = 0;
            while (t <= 1)
            {
                t += 1f/divisions;
                Vector3 newPoint = BezierCurveLerp(anchors[a - 1].anchor.position, anchors[a - 1].handles[1].position,
                    anchors[a].anchor.position, anchors[a].handles[0].position, t);
                dist += Vector3.Distance(newPoint, previousPoint);

                while (dist >= spacing)
                {
                    float overShootDistance = dist - spacing;
                    Vector3 newSpacedPoint = newPoint + (previousPoint - newPoint).normalized * overShootDistance;
                    WayPoints newWayPoint = new WayPoints();
                    newWayPoint.point = newSpacedPoint;
                    newWayPoint.up = Vector3.Lerp(anchors[a - 1].anchor.up, anchors[a].anchor.up, t).normalized;
                    newWayPoint.speed = Mathf.Lerp(anchors[a - 1].speed, anchors[a].speed, t);
                    newWayPoint.camZoom = Mathf.Lerp(anchors[a - 1].camZoom, anchors[a].camZoom, t);
                    newWayPoint.camPos = Vector3.Lerp(anchors[a - 1].camPos, anchors[a].camPos, t);
                    newWayPoint.camAngle = Quaternion.Lerp(Quaternion.Euler(anchors[a - 1].camAngle), Quaternion.Euler(anchors[a].camAngle), t);
                    evenlySpacedPoints.Add(newWayPoint);
                    dist = overShootDistance;
                    previousPoint = newSpacedPoint;
                }

                previousPoint = newPoint;

            }
        }

        if (looped)
        {
            float distanceOfOtherSegments =
                Vector3.Distance(anchors[anchors.Length-1].anchor.position, anchors[anchors.Length-1].handles[1].position) +
                Vector3.Distance(anchors[0].handles[0].position, anchors[anchors.Length-1].handles[1].position) +
                Vector3.Distance(anchors[0].handles[0].position, anchors[0].anchor.position);
            float calculatedDist = Vector3.Distance(anchors[0].anchor.position, anchors[anchors.Length-1].anchor.position) + distanceOfOtherSegments/2f;
            int divisions = Mathf.CeilToInt(calculatedDist * resolution * 10);
            float t = 0;
            while (t <= 1)
            {
                t += 1f/divisions;
                Vector3 newPoint = BezierCurveLerp(anchors[anchors.Length-1].anchor.position, anchors[anchors.Length-1].handles[1].position,
                    anchors[0].anchor.position, anchors[0].handles[0].position, t);
                dist += Vector3.Distance(newPoint, previousPoint);

                while (dist >= spacing)
                {
                    float overShootDistance = dist - spacing;
                    Vector3 newSpacedPoint = newPoint + (previousPoint - newPoint).normalized * overShootDistance;
                    WayPoints newWayPoint = new WayPoints();
                    newWayPoint.point = newSpacedPoint;
                    newWayPoint.up = Vector3.Lerp(anchors[anchors.Length - 1].anchor.up, anchors[0].anchor.up, t).normalized;
                    newWayPoint.speed = Mathf.Lerp(anchors[anchors.Length - 1].speed, anchors[0].speed, t);
                    newWayPoint.camZoom = Mathf.Lerp(anchors[anchors.Length - 1].camZoom, anchors[0].camZoom, t);
                    newWayPoint.camPos = Vector3.Lerp(anchors[anchors.Length - 1].camPos, anchors[0].camPos, t);
                    newWayPoint.camAngle = Quaternion.Lerp(Quaternion.Euler(anchors[anchors.Length - 1].camAngle), Quaternion.Euler(anchors[0].camAngle), t);
                    evenlySpacedPoints.Add(newWayPoint);
                    dist = overShootDistance;
                    previousPoint = newSpacedPoint;
                }

                previousPoint = newPoint;

            }
        }

        return evenlySpacedPoints.ToArray();
    }
}

[Serializable]
public class Anchor
{
    public Transform anchor;
    public Transform[] handles;
    public float speed;
    public Vector3 camPos;
    public Vector3 camAngle;
    public float camZoom;
}

[Serializable]
public class WayPoints
{
    public Vector3 point;
    public Vector3 up;
    public float speed;
    public Vector3 camPos;
    public Quaternion camAngle;
    public float camZoom;
}