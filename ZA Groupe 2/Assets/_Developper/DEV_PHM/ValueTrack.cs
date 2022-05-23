using System;using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeightClass
{
    NULL,
    LIGHT,
    MEDIUM,
    HEAVY,
}
public class ValueTrack : MonoBehaviour
{
    public WeightClass weightClass;
    public MeshRenderer meshRenderer;

    private void Start()
    {
        if(!meshRenderer) meshRenderer = GetComponent<MeshRenderer>();
    }
}


