using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BossRocket : MonoBehaviour
{
    public bool dice;
    public GameObject vfx;
    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = Instantiate(vfx, transform.position, quaternion.identity);
        ;
        Destroy(gameObject);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = true;   
        }
    }
}
