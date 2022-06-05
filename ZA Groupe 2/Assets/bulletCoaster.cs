using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class bulletCoaster : MonoBehaviour
{
    public GameObject explo;

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
        Destroy(Instantiate(explo, transform.position,quaternion.identity),5);
    }
}
