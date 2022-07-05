using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoueeBehavior : MonoBehaviour
{

    public float radius;
    public bool playerAboard;
    public Rigidbody rb;
    public float force;
    public Vector3 previousPos;
    public float previousangle;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,radius);
    }

    private void Start()
    {
        previousPos = transform.position;
    }

    private void Update()
    {
        
        
        if (!playerAboard)
        {
            if ((new Vector2(PlayerManager.instance.transform.position.x, PlayerManager.instance.transform.position.z) -
                 new Vector2(transform.position.x, transform.position.z)).sqrMagnitude < radius * radius)
            {
                playerAboard = true;
                
            }   
        }
        else
        {
            if ((new Vector2(PlayerManager.instance.transform.position.x, PlayerManager.instance.transform.position.z) -
                 new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > radius * radius)
            {
                PlayerManager.instance.transform.position = new Vector3(transform.position.x,
                    PlayerManager.instance.transform.position.y, transform.position.z) +
                    (PlayerManager.instance.transform.position - new Vector3(transform.position.x,
                        PlayerManager.instance.transform.position.y, transform.position.z)).normalized * radius;
            }
            

            if (PlayerManager.instance.rope.pinnedTo && PlayerManager.instance.rope.clamped)
            {
                rb.AddForceAtPosition((PlayerManager.instance.rope.pin.transform.position - PlayerManager.instance.transform.position).normalized * force,new Vector3(PlayerManager.instance.transform.position.x,transform.position.y,PlayerManager.instance.transform.position.z));
            }
            
            PlayerManager.instance.transform.position += transform.position - previousPos;
            previousPos = transform.position;
            
            
            float newAngle = transform.eulerAngles.y - previousangle;
            
            Vector3 previousPosAngle = PlayerManager.instance.transform.position - transform.position;
            
            Vector3 newPos = Quaternion.AngleAxis(newAngle, Vector3.up) * previousPosAngle;
            PlayerManager.instance.transform.position += newPos - previousPosAngle;
            previousangle = transform.eulerAngles.y;

        }
    }
}
