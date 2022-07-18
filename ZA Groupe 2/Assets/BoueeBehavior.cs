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
    public Grappin[] dockingPoints;
    public Transform[] pointToGo;
    public Grappin myGrapple;
    public int currentDock = -1;
    public WaterManager waterManager;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position,radius);
        Gizmos.color = Color.yellow;
        if(dockingPoints.Length > 0)
        {
            for (int i = 0; i < dockingPoints.Length; i++)
            {
                Gizmos.DrawWireSphere(dockingPoints[i].dock,dockingPoints[i].dockLenght);
            }
        }
    }

    private void Start()
    {
        previousPos = transform.position;
    }

    private void Update()
    {
        currentDock = -1;
        for (int i = 0; i < dockingPoints.Length; i++)
        {
            if ((dockingPoints[i].dock - transform.position).sqrMagnitude <=
                dockingPoints[i].dockLenght * dockingPoints[i].dockLenght)
            {
                currentDock = i;
                break;
            }
        }
        
        if (playerAboard)
        {
            if ((new Vector2(PlayerManager.instance.transform.position.x, PlayerManager.instance.transform.position.z) -
                 new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > radius * radius)
            {
                
                PlayerManager.instance.transform.position = new Vector3(transform.position.x,
                                                                    PlayerManager.instance.transform.position.y, transform.position.z) +
                                                                (PlayerManager.instance.transform.position - new Vector3(transform.position.x,
                                                                    PlayerManager.instance.transform.position.y, transform.position.z)).normalized * radius;
                
            }
            
            
            PlayerManager.instance.transform.position += transform.position - previousPos;
            previousPos = transform.position;
            
            
            float newAngle = transform.eulerAngles.y - previousangle;
            
            Vector3 previousPosAngle = PlayerManager.instance.transform.position - transform.position;
            
            Vector3 newPos = Quaternion.AngleAxis(newAngle, Vector3.up) * previousPosAngle;
            PlayerManager.instance.transform.position += newPos - previousPosAngle;
            previousangle = transform.eulerAngles.y;



            if (currentDock >= 0 && PlayerManager.instance.inputInteractPushed)
            {
                myGrapple.pointToGo = pointToGo[currentDock];
                playerAboard = false;
                rb.isKinematic = true;
                myGrapple.StartGrappin();
                PlayerManager.instance.inputInteractPushed = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (PlayerManager.instance.rope.pinnedTo && PlayerManager.instance.rope.clamped && playerAboard)
        {
            rb.AddForceAtPosition((PlayerManager.instance.rope.pin.transform.position - PlayerManager.instance.transform.position).normalized * force,new Vector3(PlayerManager.instance.transform.position.x,transform.position.y,PlayerManager.instance.transform.position.z));
        }
    }
}
