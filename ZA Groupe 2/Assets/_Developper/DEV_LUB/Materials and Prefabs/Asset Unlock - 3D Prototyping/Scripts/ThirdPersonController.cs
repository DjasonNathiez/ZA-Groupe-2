using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class ThirdPersonController : MonoBehaviour
{
    [FormerlySerializedAs("GameCamera")] public Camera gameCamera;
    public float playerSpeed = 2.0f;
    private float m_jumpForce = 1.0f;
    
    private CharacterController m_controller;
    private Animator m_animator;
    private Vector3 m_playerVelocity;
    private bool m_groundedPlayer;
    private float m_gravityValue = -9.81f;

    private void Start()
    {
        m_controller = gameObject.GetComponent<CharacterController>();
        m_animator = gameObject.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        m_groundedPlayer = m_controller.isGrounded;
        
        if (m_groundedPlayer && m_playerVelocity.y < 0)
        {
            m_playerVelocity.y = -0.5f;
        }

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //trasnform input into camera space
        var forward = gameCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();
        var right = Vector3.Cross(Vector3.up, forward);
        
        Vector3 move = forward * input.z + right * input.x;
        move.y = 0;
        
        m_controller.Move(move * Time.deltaTime * playerSpeed);

        m_animator.SetFloat("MovementX", input.x);
        m_animator.SetFloat("MovementZ", input.z);

        if (input != Vector3.zero)
        {
            gameObject.transform.forward = forward;
        }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && m_groundedPlayer)
        {
            m_playerVelocity.y += Mathf.Sqrt(m_jumpForce * -3.0f * m_gravityValue);
            m_animator.SetTrigger("Jump");
        }

        m_playerVelocity.y += m_gravityValue * Time.deltaTime;

        m_controller.Move(m_playerVelocity * Time.deltaTime);
    }
}
