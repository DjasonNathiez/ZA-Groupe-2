using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingPlatform : MonoBehaviour
{
    public float bounceHeight;
    public float bouncingForce;
    
    private void OnTriggerEnter(Collider col)
    {
        Rigidbody colRb = col.GetComponent<Rigidbody>();

        if (colRb)
        {
            if(colRb.GetComponent<PlayerGravity>())
            {
                //colRb.GetComponent<PlayerGravity>().BounceEffect(bounceHeight, bouncingForce);
            }
        }
    }
}
