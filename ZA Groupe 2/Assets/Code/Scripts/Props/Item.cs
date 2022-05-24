using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public float valuePercentage;
    public string affectedValue;

    public float rotationSpeed = 20f;
    public float amplitude = 0.2f;
    public float floatSpeed = 2f;

    private Vector3 possOffset;
    private Vector3 tempPos;

    void Start()
    {
        possOffset = transform.position;
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0f, Time.unscaledDeltaTime * rotationSpeed, 0f), Space.World);
        tempPos = possOffset;
        tempPos.y += Mathf.Sin(Time.fixedUnscaledTime * Mathf.PI * floatSpeed) * amplitude;

        transform.position = tempPos;
    }
}
