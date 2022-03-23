using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCurve : MonoBehaviour
{
    public float speed;
    public WayPoints[] points;
    public int currentPoint;
    public float step;
    public bool loop;

    private void FixedUpdate()
    {
        step += speed;

        while (step >= 1)
        {
            step = step - 1;
            if (currentPoint < points.Length-2)
            {
                currentPoint++;   
            }
            else if (loop)
            {
                currentPoint = 0;
            }
        }

        if (currentPoint < points.Length - 2)
        {
            transform.position = Vector3.Lerp(points[currentPoint].point, points[currentPoint + 1].point, step);
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation((points[currentPoint + 1].point - points[currentPoint].point).normalized,points[currentPoint + 1].up),
                step);
            speed = Mathf.Lerp(speed, points[currentPoint + 1].speed, step);
        }
        else if (loop)
        {
            transform.position = Vector3.Lerp(points[currentPoint].point, points[0].point, step);
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation((points[0].point - points[currentPoint].point).normalized,points[0].up),
                step);
            speed = Mathf.Lerp(speed, points[0].speed, step);
        }
    }
}
