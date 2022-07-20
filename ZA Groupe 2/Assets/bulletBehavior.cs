using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletBehavior : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    public float usedSpeed;
    public bool canBounce;
    public bool bounced;
    public Vector3 velocity;
    public AnimationCurve bounceSpeed;
    public float timeComparator;
    public Vector3 normal;
    public float bounceTime;


    private void Start()
    {
        usedSpeed = speed;
    }

    private void Update()
    {
        velocity = new Vector3(velocity.x, 0, velocity.z).normalized;
        rb.velocity = velocity * usedSpeed;
        
        if (!canBounce)
        {
            usedSpeed = Mathf.Lerp(0,speed,bounceSpeed.Evaluate((Time.time - timeComparator)/bounceTime));
            if ((Time.time - timeComparator)/bounceTime >= bounceSpeed.keys[1].time && !bounced)
            {
                velocity = Vector3.Reflect(velocity, normal);
                bounced = true;
            }
            if ((Time.time - timeComparator)/bounceTime >= bounceSpeed.keys[2].time)
            {
                Debug.Log("FinishedBounce");
                if (transform.childCount == 0)
                {
                    canBounce = true;
                    bounced = false;   
                }
            }
        }
    }

    public void Bounce(Vector3 inNormal)
    {
        Debug.Log("BALL BOUNCED");
        timeComparator = Time.time;
        canBounce = false;
        normal = inNormal;
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
