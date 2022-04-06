using System;
using System.Collections;
using DG.Tweening;
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
    public enum PlayerStateMachine { IDLE, MOVE, ATTACK, ROLLING, THROW, DEAD };

    [Header("Statistics")] 
    public float currentLifePoint;
    public float maxLifePoint;
    public bool isInvincible;

    [Header("Movement")] 
    [SerializeField] private float m_speed;
    public float moveSpeed;
    
    private Vector3 m_moveDirection;
    public float rotationSpeed;
    public bool moving;
    
    [Header("Attack")]
    public int attackDamage;
    public float attackSpeed;
    [SerializeField] private Attack m_attack;
    //Animations
    private static readonly int AttackSpeed = Animator.StringToHash("AttackSpeed");

    [Header("DistanceAttack")]
    public GameObject throwingWeapon;
    public float throwingSpeed;
    public Vector3 direction;
    public string state = "StatusQuo";
    
    [Header("Roll")]
    public float rollSpeed;
    [Range(0,1)] public float rollDuration;
    public float rollCooldown;
    private float m_rollTimer;
    [SerializeField] private bool m_canRoll;
    private bool m_isRolling;
    public AnimationCurve animationCurve;
    public float lerpTime;
    [SerializeField] private float m_acTimer;
    public GameObject pinObj;
    public GameObject pinPosBase;
    private GameObject objInFront;

    public TestRope m_rope;
    public GameObject weaponObj;

    public bool inputInterractPushed;
    public bool isMoving;
    public Vector3 move;

    private Quaternion lookRot;
    
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
        
        
        animationCurve.keys[animationCurve.length -1].time = rollDuration;
        m_acTimer = animationCurve.keys[animationCurve.length -1].time;
    }

    private void Update()
    {

        if (currentLifePoint <= 0)
        {
            playerStateMachine = PlayerStateMachine.DEAD;
        }
        
        CheckForAnimation();
        
        Cursor.visible = m_playerInput.currentControlScheme == "Keyboard&Mouse";

        switch (controlState)
        {
            case ControlState.NORMAL:
                if (playerStateMachine != PlayerStateMachine.THROW)
                {
                    m_inputController.Player.Melee.started += _ => LoadAttack();
                }
                
                m_inputController.Player.Interact.started += _ => inputInterractPushed = true;
                m_inputController.Player.Interact.canceled += _ => inputInterractPushed = false;
//        Debug.Log(state);
                switch (state)
                {
                    case "StatusQuo":
                        m_inputController.Player.Range.started += _ => Throw();
                        break;
                    case "Rope":
                        m_inputController.Player.Range.started += _ => Rewind();
                        break;
                    case "Throw":
                        throwingWeapon.transform.Translate(direction*Time.deltaTime*throwingSpeed);
                        break;
                    default: return;
                }

                if (m_canRoll)
                {
                    m_inputController.Player.Roll.started += _ => m_isRolling = true;
                }

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

                if (!m_isRolling)
                {
                    m_inputController.Player.Move.performed += context => m_moveDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
                    m_inputController.Player.Move.performed += context => move = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);

                    m_inputController.Player.Move.performed += _ => playerStateMachine = PlayerStateMachine.MOVE;
                    m_inputController.Player.Move.performed += _ => moving = true;
                }

                m_inputController.Player.Move.canceled += _ => m_moveDirection = Vector3.zero;
                m_inputController.Player.Move.canceled += _ => moving = false;

                if (!m_attack.isAttacking || !m_isRolling)
                {
                    m_inputController.Player.Move.canceled += _ => playerStateMachine = PlayerStateMachine.IDLE;
                }

                Move();
                Rotation();
                Dash();
                
                break;
            
            case ControlState.UI:
                //can't acces player controls
                break;
        }
        
        m_inputController.Player.Bugtracker.started += _ => GameManager.instance.OpenBugTrackerPanel(!GameManager.instance.bugtracker.reportPanel.activeSelf);
        m_inputController.Player.Bugtracker.started += _ => controlState = GameManager.instance.bugtracker.reportPanel.activeSelf ? ControlState.UI : ControlState.NORMAL;
        m_inputController.Player.Bugtracker.started += _ => Time.timeScale = GameManager.instance.bugtracker.reportPanel.activeSelf ? 0 : 1;

        //SET PAUSE
        
        m_inputController.Player.Pause.started += PauseOnstarted;
    }

    private void CheckForAnimation()
    {
        m_animator.SetBool("Moving", moving);
        switch (playerStateMachine)
        {
            case PlayerStateMachine.IDLE:
                m_animator.Play("Idle");
                break;
            
            case PlayerStateMachine.MOVE:

                if (!m_isRolling || !m_attack.isAttacking || state != "Throw")
                {
                    m_animator.Play("Move");
                }
                
                break;
            case PlayerStateMachine.ATTACK:
                m_animator.Play("Attack");
                break;
            case PlayerStateMachine.ROLLING:
                m_animator.Play("Roll");
                break;
            
            case PlayerStateMachine.THROW:
                m_animator.Play("Throw");
                break;
            
            case PlayerStateMachine.DEAD:
                m_animator.Play("Death");
                break;
            
        }
        
    }

    private void PauseOnstarted(InputAction.CallbackContext obj)
    {
        if (!obj.started) return;

        if (!GameManager.instance.inPause)
        {
            GameManager.instance.Pause();
            GameManager.instance.inPause = true;
        }
        else
        {
            GameManager.instance.Resume();
            GameManager.instance.inPause = false;
        }
    }

    private void Dash()
    {
        if (m_isRolling)
        {
            playerStateMachine = PlayerStateMachine.ROLLING;
            m_canRoll = false;
            
            m_acTimer -= Time.deltaTime; 
        
            m_speed = animationCurve.Evaluate(m_acTimer);
            
            if (m_acTimer <= 0)
            {
                m_speed = moveSpeed;
                m_isRolling = false;
                m_rollTimer = rollCooldown;
                m_acTimer = animationCurve.keys[animationCurve.length -1].time;
                
                playerStateMachine = !moving ? PlayerStateMachine.IDLE : PlayerStateMachine.MOVE;
            }
        }
    }

    private void PinToObject()
    {
        m_rope.pin = pinObj;

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
        //APPLY GRAVITY

        if (!m_attack.isAttacking)
        {
            m_rb.velocity = new Vector3(m_moveDirection.x * m_speed, m_rb.velocity.y, m_moveDirection.z * m_speed ) ;
        }
        else
        {
            m_rb.velocity = Vector3.zero;
        }
    }
    
    private void Throw()
    {
        if(state == "StatusQuo")
        {
            m_animator.Play("Throw");
            Debug.Log("The Throwing at " + state + "1");
            throwingWeapon.SetActive(true);
            throwingWeapon.transform.position = transform.position + transform.forward * 0.5f;
            throwingWeapon.transform.LookAt(throwingWeapon.transform.position+transform.forward);
            
            direction = Vector3.forward;
            
            state = "Throw";
            m_rope.enabled = true;
            m_rope.rope.gameObject.SetActive(true);
        }
    }
    
    public void Rewind()
    {
        if (state == "Rope")
        {
            m_rope.rewinding = true;
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
                
                        lookRot = Quaternion.LookRotation(new Vector3(aimDirection.x, 0, aimDirection.z));
                        m_rb.DORotate(lookRot.eulerAngles, 0);
                    }
                    break;
                
                case false :
                    if (m_moveDirection != Vector3.zero)
                    {
                        lookRot = Quaternion.LookRotation(m_moveDirection);
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

    public void GetHurt(int damage)
    {
        if (isInvincible) return;
        
        if (currentLifePoint > 0)
        {
            currentLifePoint -= damage;
            m_animator.Play("Hurt");
        }
        
    }

    public void RespawnPlayer()
    {
        GameManager.instance.BackToCheckpoint();
        ResetState();
        currentLifePoint = maxLifePoint;
    }

    public void ResetState()
    {
        playerStateMachine = !moving ? PlayerStateMachine.IDLE : PlayerStateMachine.MOVE;
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
