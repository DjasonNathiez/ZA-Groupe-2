using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private InputController m_inputController;
    private Rigidbody m_rb;

    [Header("Current Statistics")] 
    public float speed;

    private Vector3 moveDirection;
    
    private void Awake()
    {
        m_inputController = new InputController();
        m_inputController.Enable();
        m_rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        m_inputController.Player.Move.performed += context => moveDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
        m_inputController.Player.Move.canceled += context => moveDirection = Vector3.zero;
        
        Move();
    }

    private void Move()
    {
        m_rb.velocity = moveDirection * speed;

        if (moveDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(moveDirection); 
            m_rb.MoveRotation(lookRotation);
        }
    }
    
}
