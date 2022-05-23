using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BigWheel : MonoBehaviour
{
    public rotatingProp RotatingProp;
    float rot = 0;
    public Transform[] cabins;

    private void Update()
    {
        rot = Mathf.Lerp(rot, RotatingProp.myrotation / 6,0.2f);
        transform.rotation = Quaternion.Euler(0,0,rot);
        foreach (Transform cabin in cabins)
        {
            cabin.rotation = Quaternion.Euler(0,180,90);
        }
    }
}
