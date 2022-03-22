using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCurve : MonoBehaviour
{
    public float speed;
    public Vector3[] points;
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
            transform.position = Vector3.Lerp(points[currentPoint], points[currentPoint + 1], step);
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation((points[currentPoint + 1] - points[currentPoint]).normalized),
                5 * Time.deltaTime);
        }
        else if (loop)
        {
            transform.position = Vector3.Lerp(points[currentPoint], points[0], step);
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation((points[0] - points[currentPoint]).normalized),
                5 * Time.deltaTime);
        }
    }
}
