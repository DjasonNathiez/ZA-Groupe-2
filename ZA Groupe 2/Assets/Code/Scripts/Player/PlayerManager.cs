using System;
using System.Collections;
using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    
    private InputController m_inputController;
    public Rigidbody m_rb;
    private Animator m_animator;
    private PlayerInput m_playerInput;

    private LineRenderer m_lineRenderer;
    [SerializeField] private Vector2 mousePos;
    
    [Header("Player States")]
    public ControlState controlState;
    public enum ControlState {NORMAL, MOUNT, UI }

    public PlayerStateMachine playerStateMachine;
    public enum PlayerStateMachine { IDLE, MOVE, ATTACK, ROLLING };

    [Header("Movement")] 
    private float m_speed;
    public float moveSpeed;
    
    private Vector3 m_moveDirection;
    
    public float rotationSpeed;
    
    [Header("Attack")]
    public int attackDamage;
    public float attackSpeed;
    [SerializeField] private Attack m_attack;
    
    //Animations
    private static readonly int AttackSpeed = Animator.StringToHash("AttackSpeed");


    [Header("Roll")]
    public float rollSpeed;
    [Range(0,1)] public float rollDuration;
    public float rollCooldown;
    private float m_rollTimer;
    [SerializeField] private bool m_canRoll;
    private bool m_isRolling;
    public GameObject pinObj;
    public GameObject pinPosBase;
    
    private GameObject objInFront;

    private TestRope m_rope;

    public GameObject weaponObj;

    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        m_inputController = new InputController();
        m_animator = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody>();
        m_rope = GetComponent<TestRope>();
        m_playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        
        m_playerInput.actions = m_inputController.asset;
        m_canRoll = true;
        m_speed = moveSpeed;
    }

    private void Update()
    {
        Cursor.visible = m_playerInput.currentControlScheme == "Keyboard&Mouse";
        m_inputController.Player.Melee.started += _ => LoadAttack();

        if (objInFront)
        {
            m_inputController.Player.Rope.started += _ => PinToObject();
        }

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

            m_inputController.Player.MousePosition.performed += context => mousePos = context.ReadValue<Vector2>();

            m_inputController.Player.Move.performed += context => m_moveDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
            m_inputController.Player.Move.canceled += _ => m_moveDirection = Vector3.zero;
            
            Move();
            Rotation();
    }

    IEnumerator Dash()
    {
        if (m_canRoll)
        {
            m_isRolling = true;

            m_speed = rollSpeed;
        
            yield return new WaitForSeconds(rollDuration);
        
            m_rollTimer = rollCooldown;

            m_speed = moveSpeed;
            m_isRolling = false;
        }
    }

    private void PinToObject()
    {
        m_rope.pin = pinObj;
        
        m_rope.isPinned = !m_rope.isPinned;
        
        pinObj.transform.SetParent(objInFront.transform);
        pinObj.transform.position = objInFront.GetComponent<Pinnable>().pinPoint.transform.position;

        if (objInFront.GetComponent<Pinnable>().canBeGrab)
        {
            m_rope.pinnedToObject = true;
            m_rope.pinnedRb = objInFront.GetComponent<Rigidbody>();
            m_rope.pinnedObjectDistance = Vector3.Distance(transform.position, pinObj.transform.position);
        }
        m_lineRenderer.enabled = true;
    }

    public void SetFrontObject(GameObject detectedObj)
    {
        objInFront = detectedObj;
    }
    
    private void Move()
    {
        if (!m_attack.isAttacking)
        {
            m_rb.velocity = m_moveDirection * m_speed;
        }
        else
        {
            m_rb.velocity = Vector3.zero;
        }
    }

    private void LoadAttack()
    {
        playerStateMachine = PlayerStateMachine.ATTACK;
        m_attack.isAttacking = true;
        m_animator.SetFloat(AttackSpeed, attackSpeed);
        m_animator.Play("attack_first");
    }

    private void Rotation()
    {
        if (m_playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            switch (m_attack.isAttacking)
            {
                case true :
                    if (Camera.main)
                    {
                        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mousePos);
                        Vector3 aimDirection = (mousePosition - transform.position).normalized;
                
                        Quaternion lookRot = Quaternion.LookRotation(new Vector3(aimDirection.x, 0, aimDirection.z));
                        m_rb.DORotate(lookRot.eulerAngles, 0);
                    }
                    break;
                
                case false :
                    if (m_moveDirection != Vector3.zero)
                    {
                        Quaternion lookRot = Quaternion.LookRotation(m_moveDirection);
                        m_rb.DORotate(lookRot.eulerAngles, rotationSpeed);
                    }
                    break;
            }
        }
        else
        {
            if (m_moveDirection != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(m_moveDirection);
                m_rb.MoveRotation(lookRotation);
            }
        }
    }

    public void ResetState()
    {
        playerStateMachine = PlayerStateMachine.IDLE;
        m_attack.isAttacking = false;
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
