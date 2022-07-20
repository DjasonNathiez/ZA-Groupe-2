using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletBehavior : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    public bool canBounce;
    public float timer;
    public float delay;


    private void Update()
    {
        rb.velocity = new Vector3(rb.velocity.x,0,rb.velocity.z) * speed;
        if (!canBounce)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = delay;
                canBounce = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("UngrippableObject") || other.CompareTag("GrippableObject") ||
                 other.CompareTag("TractableObject") || other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
