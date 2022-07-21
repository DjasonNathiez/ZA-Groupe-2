using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TourelleBehaviour : MonoBehaviour
{
    private Transform player;
    [SerializeField] private Transform canon;
    [SerializeField] private Transform followTransform;

    [SerializeField] private TourelleState currentState;

    [SerializeField] private float detectionRange;
    
    [SerializeField] private float detectionDuration;
    private float detectionTimer;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private float followDuration;
    private float followTimer;

    [SerializeField] private float beforeShootDuration;
    private float beforeShootTimer;
    
    public GameObject currentBullet;

    [SerializeField] private ParticleSystem shootVFX;
    
    public enum TourelleState
    {
        Idle, Detection, Follow, Shoot, Wait
    }

    private void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        player = PlayerManager.instance.transform;
        SwitchState(TourelleState.Idle);
    }

    private void Update()
    {
        CheckState();
    }

    private void CheckState()
    {
        switch (currentState)
        {
            case TourelleState.Idle:
                
                // Check Player Distance
                var distance = Vector3.Distance(player.position, transform.position);
                if (distance <= detectionRange)
                {
                    SwitchState(TourelleState.Detection);
                }

                break;
            
            case TourelleState.Detection:

                if (detectionTimer >= detectionDuration)
                {
                    SwitchState(TourelleState.Follow);
                }
                else
                {
                    detectionTimer += Time.deltaTime;
                }
                
                // Timer
                
                break;
            
            case TourelleState.Follow:

                followTransform.LookAt(player.position);

                canon.rotation = Quaternion.RotateTowards(transform.rotation, followTransform.rotation, 360);
                
                canon.eulerAngles = new Vector3(0, canon.eulerAngles.y, 0);
                
                if (followTimer >= followDuration)
                {
                    SwitchState(TourelleState.Shoot);
                }
                else
                {
                    followTimer += Time.deltaTime;
                }
                // Timer
                
                break;
            
            case TourelleState.Shoot:
                
                // Lance une fois le bullet
                if (beforeShootTimer >= beforeShootDuration)
                {
                    Debug.Log("Shoot bullet !");
                    
                    shootVFX.Play();
                
                    SwitchState(TourelleState.Wait);
                }
                else
                {
                    beforeShootTimer += Time.deltaTime;
                }
                
                break;
            
            case TourelleState.Wait:

                if (currentBullet) return;
                
                SwitchState(TourelleState.Idle);
                // Attend destruction du bullet lancé
                
                break;
        }
    }

    private void SwitchState(TourelleState state)
    {
        switch (state)
        {
            case TourelleState.Idle:
                
                break;
            
            case TourelleState.Detection:

                detectionTimer = 0f;
                break;
            
            case TourelleState.Follow:

                followTimer = 0f;
                break;
            
            case TourelleState.Shoot:
                
                break;
            
            case TourelleState.Wait:
                
                break;
        }

        Debug.Log("Tourelle est désormais en " + state);
        currentState = state;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
