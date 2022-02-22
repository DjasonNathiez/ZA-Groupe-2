using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private InputController m_inputController;
    private Rigidbody m_rb;
    private Animator m_animator;
    private PlayerInput m_playerInput;


    private Vector2 mousePos;
    
    [Header("Player States")]
    public ControlState controlState;
    public enum ControlState {NORMAL, MOUNT, UI }

    public PlayerStateMachine playerStateMachine;
    public enum PlayerStateMachine { IDLE, MOVE, ATTACK, ROLLING };

    [Header("Movement")] 
    private float m_speed;
    public float moveSpeed;
    
    private Vector3 m_moveDirection;
    
    [Header("Attack")]
    public int attackDamage;
    public float attackSpeed;

    //Animations
    private static readonly int AttackSpeed = Animator.StringToHash("AttackSpeed");


    [Header("Roll")]
    public float rollSpeed;
    [Range(0,1)] public float rollDuration;
    public float rollCooldown;
    private float m_rollTimer;
    private bool m_canRoll;
    private bool m_isRolling;


    private void Awake()
    {
        m_inputController = new InputController();
        m_animator = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody>();
        m_playerInput = GetComponent<PlayerInput>();

        m_playerInput.actions = m_inputController.asset;
        
        m_speed = moveSpeed;
    }

    private void Update()
    {
        m_inputController.Player.Move.canceled += _ => m_moveDirection = Vector3.zero;
      
        m_inputController.Player.Move.performed += context => m_moveDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
        m_inputController.Player.Melee.started += _ => LoadAttack();
        
        
        
        m_inputController.Player.Roll.started += _ => StartCoroutine(Dash());

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

        if (!m_isRolling)
        {
            NormalMove();
        }

    }

    IEnumerator Dash()
    {
        if (m_canRoll)
        {
            m_isRolling = true;
            m_canRoll = false;

            m_speed = rollSpeed;
        
            yield return new WaitForSeconds(rollDuration);
        
            m_rollTimer = rollCooldown;

            m_speed = moveSpeed;
            m_isRolling = false;
        }
    }


    private void NormalMove()
    { 
        //le joueur se déplace dans la direction où le joystick est dirigé avec une vitesse donné
        m_rb.velocity = m_moveDirection * m_speed;
        
        
            //le joueur regarde dans la direction où il se déplace
            var lookRotation = Quaternion.LookRotation(m_moveDirection);
            m_rb.MoveRotation(lookRotation);
        
        

    }

    private void LoadAttack()
    {
        Debug.Log(m_playerInput.currentControlScheme);

        attackDamage = 1;
        
        m_animator.SetFloat(AttackSpeed, attackSpeed);
        m_animator.Play("attack_first");
    }

    public void ResetState()
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
