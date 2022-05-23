using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockableObject : MonoBehaviour
{
    public Rigidbody rb;
    public float force;
    public float yforce;
    public bool isHit = true;

    public GameObject popcornInterior;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
}
