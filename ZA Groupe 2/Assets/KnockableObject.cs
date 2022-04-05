using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockableObject : MonoBehaviour
{
    public Rigidbody rb;
    public float force;
    public float yforce;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
}
