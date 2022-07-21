using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    public GameObject vfxImpact;


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
                GameObject vfx = Instantiate(vfxImpact, transform.position, quaternion.identity);
                vfx.transform.LookAt(vfx.transform.position - normal);
                Destroy(vfx,2);
            }
            if ((Time.time - timeComparator)/bounceTime >= bounceSpeed.keys[2].time)
            {
                if (transform.childCount == 0)
                {
                    Debug.Log("ENDED BOUNCE "+ Time.time);
                    canBounce = true;
                    bounced = false;   
                }
            }
        }
    }

    public void Bounce(Vector3 inNormal)
    {
        Debug.Log("ANNOUNCED");
        if (canBounce)
        {
            Debug.Log("BALL BOUNCED "+ Time.time);
            timeComparator = Time.time;
            canBounce = false;
            normal = inNormal; 
            Debug.DrawRay(transform.position,normal,Color.black,3);
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
