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
    public bool hasBounced;
    public bool isElectrified;
    public Color notElectrified;
    public Color electrified;
    
    private void Start()
    {
        usedSpeed = speed;
        Destroy(gameObject, 5f);
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
                hasBounced = true;
                GameObject vfx = Instantiate(vfxImpact, transform.position, quaternion.identity);
                vfx.transform.LookAt(vfx.transform.position - normal);
                Destroy(vfx,2);
            }
            if ((Time.time - timeComparator)/bounceTime >= bounceSpeed.keys[2].time)
            {
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
        if (canBounce)
        {
            timeComparator = Time.time;
            canBounce = false;
            if (PlayerManager.instance.rope.electrocuted)
            {
                isElectrified = true;
                GetComponent<Renderer>().material.SetColor("_EmissionColor", electrified);
            }
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
            if (other.GetComponent<Taupe>())
            {
                other.GetComponent<Taupe>().TaupeHit(false);
            }
            
            if(other.GetComponent<ValueTrack>() && other.GetComponent<ValueTrack>().projectileDestroyed) Destroy(other.gameObject);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Pilone"))
        {
            if (isElectrified)
            {
                other.GetComponent<ElectrocutedProp>().LightsOn();
            }
            GetComponent<Renderer>().material.SetColor("_EmissionColor", notElectrified);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Boss") && hasBounced)
        {
            if (other.GetComponent<BossBehaviour>().doingTornado)
            {
                other.GetComponent<BossBehaviour>().StartCoroutine(other.GetComponent<BossBehaviour>().Fall(velocity));
                Destroy(gameObject);   
            }
        }
    }
}
