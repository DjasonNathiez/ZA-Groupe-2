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
    public Material material;

    private void Start()
    {
        if(!material) material = GetComponent<MeshRenderer>().material;
    }
}


