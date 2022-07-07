using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
public class QuadraticCurve : MonoBehaviour
{
    public Anchor[] anchors;
    public float spacing = 1;
    public float resolution = 1;
    public bool looped;
    public WayPoints[] points;
    public FollowCurve followCurve;
    public Vector3[] meshPointCoord;
    public Vector3[] meshPlankCoord;
    public int[] plankInts;
    public MeshFilter meshFilter;
    [HideInInspector] public bool bake;

    private void Start()
    {
        points = EvenlySpacedPoints(spacing, resolution);

        followCurve.points = points;
        followCurve.transform.position = followCurve.points[0].point;
        meshFilter.mesh = CreateMesh();
    }


    private void OnDrawGizmos()
    {
        if (spacing > 0)
        {
            points = EvenlySpacedPoints(spacing, resolution);
            foreach (WayPoints pos in points)
            {
                Gizmos.DrawSphere(pos.point,0.25f);
                Gizmos.DrawRay(pos.point,pos.up*2);
            }   
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



    public Mesh CreateMesh()
    {
        List<Vector3> meshPoints = new List<Vector3>(0);
        List<int> meshTriangles = new List<int>(0);
        
        int x = meshPointCoord.Length;
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 tangent = Vector3.zero;
            Vector3 binormal = Vector3.zero;
            Vector3 normal = Vector3.zero;
            if (i > 0)
            {
                 tangent = default;
                 binormal = points[i].up;
                 normal = (points[i].point - points[i - 1].point).normalized;
                Vector3.OrthoNormalize(ref binormal,ref normal,ref tangent);
                Debug.DrawRay(points[i].point,normal,Color.red,100);
                Debug.DrawRay(points[i].point,binormal,Color.green,100);
                Debug.DrawRay(points[i].point,tangent,Color.blue,100);
            }
            else
            {
                tangent = default;
                binormal = points[i].up;
                normal = (points[i+1].point - points[i].point).normalized;
                Vector3.OrthoNormalize(ref binormal,ref normal,ref tangent);
                Debug.DrawRay(points[i].point,normal,Color.yellow,100);
                Debug.DrawRay(points[i].point,binormal,Color.magenta,100);
                Debug.DrawRay(points[i].point,tangent,Color.cyan,100);
            }





            for (int j = 0; j < x; j++)
            {
                Vector3 point;
                point = points[i].point + tangent * meshPointCoord[j].x + binormal * meshPointCoord[j].y + normal * meshPointCoord[j].z;
                
                meshPoints.Add(point);
            }

            if (i > 0)
            {
                for (int j = 0; j < x; j++)
                {
                    meshTriangles.Add( (i-1)*x + ((j +1)% x));

                    meshTriangles.Add( (i-1)*x + j + x);
                    
                    meshTriangles.Add( i*x + (j+1)%x);
                    
                    
                    meshTriangles.Add( (i-1)*x + j );

                    meshTriangles.Add( (i-1)*x + j + x);
                    
                    meshTriangles.Add( (i-1)*x + ((j +1)% x));
                }
            }
        }

        int y = points.Length - 1;
        
        for (int j = 0; j < x; j++)
        {
            meshTriangles.Add( y * x + ((j +1)% x));
        
            meshTriangles.Add( y * x + j);

            meshTriangles.Add( 0 + j);
                    
                    
            meshTriangles.Add( 0 + j);

            meshTriangles.Add( ((j +1)% x));
                    
            meshTriangles.Add( y * x + ((j +1)% x));
        }

        y = meshPoints.Count - 1;


        int pos = meshPoints.Count;
        
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 tangent = Vector3.zero;
            Vector3 binormal = Vector3.zero;
            Vector3 normal = Vector3.zero;
            if (i > 0)
            {
                tangent = default;
                binormal = points[i].up;
                normal = (points[i].point - points[i - 1].point).normalized;
                Vector3.OrthoNormalize(ref binormal,ref normal,ref tangent);
                Debug.DrawRay(points[i].point,normal,Color.red,100);
                Debug.DrawRay(points[i].point,binormal,Color.green,100);
                Debug.DrawRay(points[i].point,tangent,Color.blue,100);
            }
            else
            {
                tangent = default;
                binormal = points[i].up;
                normal = (points[i+1].point - points[i].point).normalized;
                Vector3.OrthoNormalize(ref binormal,ref normal,ref tangent);
                Debug.DrawRay(points[i].point,normal,Color.red,100);
                Debug.DrawRay(points[i].point,binormal,Color.green,100);
                Debug.DrawRay(points[i].point,tangent,Color.blue,100);
            }





            for (int j = 0; j < x; j++)
            {
                Vector3 point;
                point = points[i].point + tangent * -meshPointCoord[j].x + binormal * meshPointCoord[j].y + normal * meshPointCoord[j].z;
                meshPoints.Add(point);
            }

            if (i > 0)
            {
                for (int j = 0; j < x; j++)
                {
                    meshTriangles.Add( pos + ((i-1)*x + ((j +1)% x)));

                    meshTriangles.Add( pos + (i*x + (j+1)%x));
                    
                    meshTriangles.Add( pos + ((i-1)*x + j + x));


                    meshTriangles.Add( pos + ((i-1)*x + j ));

                    meshTriangles.Add( pos + ((i-1)*x + ((j +1)% x)));
                    
                    meshTriangles.Add( pos + ((i-1)*x + j + x));
                    
                }
            }
        }
        
        
        int z = points.Length * 2 - 1;

        for (int j = 0; j < x; j++)
        {
            meshTriangles.Add( z * x + j);
            
            meshTriangles.Add( z * x + ((j +1)% x));

            meshTriangles.Add( y + 1 + j);
                    
            
            meshTriangles.Add( y + 1 + ((j +1)% x));
                    
            meshTriangles.Add( y + 1 + j);

            meshTriangles.Add( z * x + ((j +1)% x));
        }
        
        
        
        Mesh mesh = new Mesh();
        mesh.vertices = meshPoints.ToArray();
        mesh.triangles = meshTriangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        return mesh;
    }
    
    
    public Mesh CreateQuad()
    {
        List<Vector3> meshPoints = new List<Vector3>(0);
        List<int> meshTriangles = new List<int>(0);
        int y = 0;
        int x = meshPlankCoord.Length;
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 tangent = Vector3.zero;
            Vector3 binormal = Vector3.zero;
            Vector3 normal = Vector3.zero;
            if (i > 0)
            {
                tangent = default;
                binormal = points[i].up;
                normal = (points[i].point - points[i - 1].point).normalized;
                Vector3.OrthoNormalize(ref binormal,ref normal,ref tangent);
                Debug.DrawRay(points[i].point,normal,Color.red,100);
                Debug.DrawRay(points[i].point,binormal,Color.green,100);
                Debug.DrawRay(points[i].point,tangent,Color.blue,100);
            }




            if (i % 2 == 0)
            {

                for (int j = 0; j < x; j++)
                {
                    Vector3 point;
                    point = points[i].point + tangent * meshPlankCoord[j].x + binormal * meshPlankCoord[j].y + normal * meshPlankCoord[j].z;
                
                    meshPoints.Add(point);
                }


                foreach (int z in plankInts)
                {
                    meshTriangles.Add(z + (y * meshPlankCoord.Length));
                }

                y++;
            }
        }
        Mesh mesh = new Mesh();
        mesh.vertices = meshPoints.ToArray();
        mesh.triangles = meshTriangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        return mesh;
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