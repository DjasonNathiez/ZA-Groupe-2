using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
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
    public enum PlayerStateMachine { IDLE, MOVE, ATTACK, ROLLING };

    [Header("Current Statistics")] 
    public float speed;

    public float moveSpeed;
    public int attackDamage;
    public float attackSpeed;

    public float rollSpeed;
    [Range(0,1)] public float rollDuration;
    public float rollCooldown;
    [SerializeField]  private float m_rollTimer;
    [SerializeField]  private bool m_canRoll;
    [SerializeField] private bool m_isRolling;
    
    private Vector3 m_moveDirection;

    private void Awake()
    {
        m_inputController = new InputController();
        m_animator = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
       
        m_inputController.Player.Move.canceled += context => m_moveDirection = Vector3.zero;
        if (!m_isRolling)
        {
            m_inputController.Player.Move.performed += context => m_moveDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
            m_inputController.Player.Melee.started += context => LoadAttack();
        }

        
        m_inputController.Player.Roll.started += context => StartCoroutine(Dash());
        
        
        switch (m_rollTimer)
        {
            case > 0:
                m_rollTimer -= Time.deltaTime;
                m_canRoll = false;
                break;
            case <= 0:
                m_canRoll = true;
                break;
        }

        NormalMove();

    }

    IEnumerator Dash()
    {
        if (m_canRoll)
        {
            m_isRolling = true;
            m_canRoll = false;

            speed = rollSpeed;
        
            yield return new WaitForSeconds(rollDuration);
        
            m_rollTimer = rollCooldown;

            speed = moveSpeed;
            m_isRolling = false;
        }
    }


    private void NormalMove()
    { 
        m_rb.velocity = m_moveDirection * speed;

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
        
        m_animator.SetFloat("AttackSpeed", attackSpeed);
        m_animator.Play("attack_first");
    }

    private void ResetState()
    {
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
