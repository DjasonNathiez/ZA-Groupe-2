using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    public CapsuleCollider capsuleCollider;
    public Rigidbody rb;

    public bool isBouncing;

    [Header("Ground Check")]
    public bool playerIsGrounded = true;
    [Range(0.0f, 1.0f)] public float groundCheckRadiusMultiplier = 0.9f;
    [Range(-0.95f, 1.05f)] public float groundCheckDistance = 0.05f;
    private RaycastHit _groundCheckHit = new RaycastHit();
    public GameObject stepRayUpper;
    public GameObject stepRayLower;
    public float stepHeight = 0.3f;
    public float stepSmooth = 0.1f;

    [Header("Gravity")] 
    public float gravityFallCurrent = -100.0f;
    public float gravityFallMin = -100.0f;
    public float gravityFallMax = -500.0f;
    /*[Range(-5.0f, -35.0f)]*/ public float gravityFallIncrementAmount = -20.0f;
    public float gravityFallIncrementTime = 0.05f;
    public float playerFallTimer = 0.0f;
    public float gravity = 0.0f;

    [Header("Bouncing")] 
    public float bounceTime;
    private float bounceInTimer;
    private bool reachTheSky;
    
    private Vector3 bounceDir;

    private void Awake()
    {
        var position = stepRayUpper.transform.position;
        position = new Vector3(position.x, stepHeight, position.z);
        stepRayUpper.transform.position = position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(rb.position, (capsuleCollider.bounds.extents.y - (capsuleCollider.radius * groundCheckRadiusMultiplier)) + groundCheckDistance);
    }

    private void FixedUpdate()
    {
        playerIsGrounded = PlayerGroundCheck();
        rb.velocity = new Vector3(rb.velocity.x, PlayerGravityMethod() ,rb.velocity.z);

        Debug.Log(DistanceToGround());
        
        if (isBouncing)
        {
            if(DistanceToGround() < 4 && reachTheSky == false)
            {
                rb.AddForce(bounceDir, ForceMode.Impulse);
                bounceInTimer -= Time.deltaTime;
                gravity = 0;

                if (DistanceToGround() >= 3.99)
                {
                    reachTheSky = true;
                }
                
            }
            
            if (reachTheSky)
            {
                gravity += -5 * Time.deltaTime;
                
                if (DistanceToGround() <= 1)
                {
                    reachTheSky = false;
                    isBouncing = false;
                }
            }
            
        }
        
        StepClimb();
    }

    private bool PlayerGroundCheck()
    {
        return DistanceToGround() < 0.8;
    }

    private float DistanceToGround()
    {
        RaycastHit groundHit;

        if (Physics.Raycast(rb.position, Vector3.down, out groundHit, 1000))
        {
            float distanceToGround = Vector3.Distance(transform.position, groundHit.point);
            return distanceToGround;
        }
        Debug.DrawLine(rb.position, Vector3.down);

        return 0;
    }

    private float PlayerGravityMethod()
    {
        if (playerIsGrounded)
        {
            gravity = 0.0f;
            gravityFallCurrent = gravityFallMin; 
        }
        else
        {
            if (!isBouncing)
            {
                playerFallTimer -= Time.fixedDeltaTime;
                if (playerFallTimer < 0.0f)
                {
                    if (gravityFallCurrent < gravityFallMax)
                    {
                        gravityFallCurrent += gravityFallIncrementAmount;
                    }
                    playerFallTimer = gravityFallIncrementTime;
                    gravity = gravityFallCurrent;
                }
            }
        }

        return gravity;
    }

    private void StepClimb()
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward),
                out hitUpper, 0.2f))
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
        }
    }

    public void BounceEffect(float bHeight, float bForce)
    {
        gravityFallCurrent = -50;
        bounceInTimer = bounceTime;
        isBouncing = true;
        bounceDir = new Vector3(0, bHeight * bForce, 0);
    }

}
