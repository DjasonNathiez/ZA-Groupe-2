using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private InputController m_inputController;
    public Rigidbody m_rb;
    private Animator m_animator;
    private PlayerInput m_playerInput;

    private LineRenderer m_lineRenderer;
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
    [SerializeField] private bool m_canRoll;
    private bool m_isRolling;
    public GameObject pinObj;
    public GameObject pinPosBase;
    
    private GameObject objInFront;

    private TestRope m_rope;

    public GameObject weaponObj;

    private void Awake()
    {
        m_inputController = new InputController();
        m_animator = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody>();
        m_rope = GetComponent<TestRope>();
        m_playerInput = GetComponent<PlayerInput>();

        m_playerInput.actions = m_inputController.asset;
        m_canRoll = true;
        m_speed = moveSpeed;
    }

    private void Update()
    {
        m_inputController.Player.Move.canceled += _ => m_moveDirection = Vector3.zero;
      
        m_inputController.Player.Move.performed += context => m_moveDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
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
        
        NormalMove();
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
    
    private void NormalMove()
    { 
        //le joueur se déplace dans la direction où le joystick est dirigé avec une vitesse donné
        m_rb.velocity = m_moveDirection * m_speed;

        //le joueur regarde dans la direction où il se déplace
        if (m_moveDirection != Vector3.zero)
        {
                Quaternion lookRotation = Quaternion.LookRotation(m_moveDirection);
                m_rb.MoveRotation(lookRotation);
        }
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
