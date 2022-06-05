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
    [Header("Usefull Attributs")]
    public bool isEnemy;
    public bool canActivatePressurePlate;
    
    [Header("Outline Attributs")]
    public WeightClass weightClass;
    public MeshRenderer meshRenderer;
    public Vector3 posCam;
    public Vector3 rotCam;
    public bool moveCam;

    private void Start()
    {
        if(!meshRenderer) meshRenderer = GetComponent<MeshRenderer>();
    }
}


