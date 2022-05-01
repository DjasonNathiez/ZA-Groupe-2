using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lumiereActivation : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public rotatingProp rotatingProp;
    public float angle;
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (rotatingProp.myrotation > angle || rotatingProp.myrotation < -angle)
        {
            meshRenderer.enabled = false;
        }
        else
        {
            meshRenderer.enabled = true;
        }
    }
}
