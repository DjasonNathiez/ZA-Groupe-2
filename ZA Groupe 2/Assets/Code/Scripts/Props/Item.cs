using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public float valuePercentage;
    public string affectedValue;
    public bool distributed;

    public float rotationSpeed = 20f;
    public float amplitude = 0.2f;
    public float floatSpeed = 2f;
    public float height;

    private Vector3 possOffset;
    private Vector3 tempPos;
    public PnjDialoguesManager PnjDialoguesManager;

    void Start()
    {
        possOffset = transform.position;
        PnjDialoguesManager.dialogue[0].positionCamera = possOffset;
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0f, Time.unscaledDeltaTime * rotationSpeed, 0f), Space.World);
        tempPos = possOffset;
        tempPos.y += Mathf.Sin(Time.fixedUnscaledTime * Mathf.PI * floatSpeed) * amplitude;

        transform.position = Vector3.Lerp(transform.position, possOffset + Vector3.up * height, 5 * Time.deltaTime);
    }
}
