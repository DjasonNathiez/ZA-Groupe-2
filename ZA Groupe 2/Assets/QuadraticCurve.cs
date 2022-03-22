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
    public Vector3[] points;
    public FollowCurve followCurve;

    private void Start()
    {
        points = EvenlySpacedPoints(spacing, resolution);

        followCurve.points = points;
    }


    private void OnDrawGizmos()
    {
        points = EvenlySpacedPoints(spacing, resolution);
        foreach (Vector3 pos in points)
        {
            Gizmos.DrawSphere(pos,0.25f);
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

    public Vector3[] EvenlySpacedPoints(float spacing, float resolution)
    {
        List<Vector3> evenlySpacedPoints = new List<Vector3>();
        evenlySpacedPoints.Add(anchors[0].anchor.position);
        Vector3 previousPoint = evenlySpacedPoints[0];
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
                    evenlySpacedPoints.Add(newSpacedPoint);
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
                    evenlySpacedPoints.Add(newSpacedPoint);
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
}