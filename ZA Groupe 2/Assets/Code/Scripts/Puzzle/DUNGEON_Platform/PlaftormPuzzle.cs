using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaftormPuzzle : MonoBehaviour
{
    public bool moveX;
    public bool moveZ;
    public float speed;
    public float amplitude;

    private Vector3 originPos;
    private Vector3 tempPos;

    private void Awake()
    {
        originPos = transform.position;
    }

    private void Update()
    {
        if (moveX)
        {
            tempPos = originPos;
            tempPos.x =  originPos.x + Mathf.Sin(Time.fixedTime * Mathf.PI * speed) * amplitude;

            transform.position = tempPos;
        }

        if (moveZ)
        {
            tempPos = originPos;
            tempPos.z =  originPos.z + Mathf.Sin(Time.fixedTime * Mathf.PI * speed) * amplitude;

            transform.position = tempPos;
        }
    }
}
