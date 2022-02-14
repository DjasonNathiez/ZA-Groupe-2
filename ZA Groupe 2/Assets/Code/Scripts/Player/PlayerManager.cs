using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private InputController m_inputController;
    private Rigidbody m_rb;
    private Animator m_animator;

    public ControlerState controlerState;
    public enum ControlerState
    {
        NORMAL,
        MOUNT,
        UI
    }

    public PlayerStateMachine playerStateMachine;
    public enum PlayerStateMachine { IDLE, MOVE, ATTACK };

    [Header("Current Statistics")] 
    public float speed;
    public int attackDamage;
    public float attackSpeed;
    
    private Vector3 m_moveDirection;

    private void Awake()
    {
        m_inputController = new InputController();
        m_animator = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        m_inputController.Player.Move.performed += context => m_moveDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
        m_inputController.Player.Move.canceled += context => m_moveDirection = Vector3.zero;

        m_inputController.Player.Melee.started += context => LoadAttack();
        
        NormalMove();

    }
    
    
    private void NormalMove()
    {
        if (playerStateMachine != PlayerStateMachine.ATTACK)
        {
            m_rb.velocity = m_moveDirection * speed;
            playerStateMachine = PlayerStateMachine.MOVE;
        }
        
        if (m_moveDirection != Vector3.zero)
        { 
           
            //change the look direction to the last move direction
            Quaternion lookRotation = Quaternion.LookRotation(m_moveDirection); 
            m_rb.MoveRotation(lookRotation);
            
            //switch the state of player to MOVE, which means the player is moving
            
        }
        
    }

    private void LoadAttack()
    {
        Debug.Log("ATTACK !");
        attackDamage = 1;
        
        playerStateMachine = PlayerStateMachine.ATTACK;
        
        m_animator.SetFloat("AttackSpeed", attackSpeed);
        m_animator.Play("attack_first");
    }

    private void ResetState()
    {
        playerStateMachine = PlayerStateMachine.IDLE;
    }
    

    private void OnEnable()
    {
        m_inputController.Enable();
    }

    private void OnDisable()
    {
        m_inputController.Disable();
    }
}
