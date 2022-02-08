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
    
    public PlayerStateMachine playerStateMachine;
    public enum PlayerStateMachine { IDLE, MOVE, ATTACK };
    
    [Header("Current Statistics")] 
    public float speed;
    private Vector3 m_moveDirection;
    
    private void Awake()
    {
        m_inputController = new InputController();
        m_inputController.Enable();
        m_rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        m_inputController.Player.Move.performed += context => m_moveDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
        m_inputController.Player.Move.canceled += context => m_moveDirection = Vector3.zero;
        
        Move();
    }

    private void Move()
    {
        //move to the direction of the input by movement speed
        m_rb.velocity = m_moveDirection * speed;

        if (m_moveDirection != Vector3.zero)
        {
            //change the look direction to the last move direction
            Quaternion lookRotation = Quaternion.LookRotation(m_moveDirection); 
            m_rb.MoveRotation(lookRotation);
            
            //switch the state of player to MOVE, which means the player is moving
            playerStateMachine = PlayerStateMachine.MOVE;
        }
        else
        {
            //in case that the moveDirection Vector is equal to zero, this mean the player isn't moving so he back to the idle state
            playerStateMachine = PlayerStateMachine.IDLE;
        }
    }
    
}
