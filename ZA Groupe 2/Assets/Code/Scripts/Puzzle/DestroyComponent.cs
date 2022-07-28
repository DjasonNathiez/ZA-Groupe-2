using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyComponent : MonoBehaviour
{
    public Vector3 teleportPoint;
    public GameObject plateforme;
    public Vector3 position;
    public rotatingProp theCrank;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        other.transform.position = teleportPoint;
        if (plateforme == null) return;
        plateforme.transform.position = position;
        theCrank.myrotation = 0;
    }
}
