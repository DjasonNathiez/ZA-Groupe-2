using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRocket : MonoBehaviour
{
    public bool dice;
    private void OnTriggerEnter(Collider other)
    {
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
