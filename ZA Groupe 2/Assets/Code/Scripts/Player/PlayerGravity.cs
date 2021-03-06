using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    public CapsuleCollider capsuleCollider;
    public Rigidbody rb;

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
    [Range(-5.0f, -35.0f)] public float gravityFallIncrementAmount = -20.0f;
    public float gravityFallIncrementTime = 0.05f;
    public float playerFallTimer = 0.0f;
    public float gravity = 0.0f;

    private void Awake()
    {
        var position = stepRayUpper.transform.position;
        position = new Vector3(position.x, stepHeight, position.z);
        stepRayUpper.transform.position = position;
    }

    private void FixedUpdate()
    {
        playerIsGrounded = PlayerGroundCheck();
        rb.velocity = new Vector3(rb.velocity.x, PlayerGravityMethod() ,rb.velocity.z);
        
        StepClimb();
    }

    private bool PlayerGroundCheck()
    {
        float sphereCastRadius = capsuleCollider.radius * groundCheckRadiusMultiplier;
        float sphereCastTravelDistance = capsuleCollider.bounds.extents.y - sphereCastRadius + groundCheckDistance;
        return Physics.SphereCast(rb.position, sphereCastRadius, Vector3.down, out _groundCheckHit,
            sphereCastTravelDistance);
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
            playerFallTimer -= Time.fixedDeltaTime;
            if (playerFallTimer < 0.0f)
            {
                if (gravityFallCurrent > gravityFallMax)
                {
                    gravityFallCurrent += gravityFallIncrementAmount;
                }
                playerFallTimer = gravityFallIncrementTime;
                gravity = gravityFallCurrent;
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

}
