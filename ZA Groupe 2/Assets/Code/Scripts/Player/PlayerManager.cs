using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[SuppressMessage("ReSharper", "CheckNamespace")]
public class PlayerManager : MonoBehaviour
{

    public GameObject Poufpouf;

    public static PlayerManager instance; //Singleton

    #region Components

    //Unity components 
    private InputController m_inputController;
    [HideInInspector] public Rigidbody rb;
    private Animator m_animator;
    private PlayerInput m_playerInput;
    private Vector2 m_mousePos;

    //Personal scripts components
    private Attack m_attack;
    [HideInInspector] public Rope rope;

    #endregion

    #region Player States

    private ControlState m_controlState;

    private enum ControlState
    {
        NORMAL,
        UI,
        DIALOGUE
    }

    private PlayerStateMachine m_playerStateMachine;

    private enum PlayerStateMachine
    {
        IDLE,
        MOVE,
        ATTACK,
        ROLLING,
        THROW,
        STUN,
        DEAD
    };

    #endregion

    #region Public variables

    [Header("States Stats")] 
    public float baseLifePoint;
    [HideInInspector] public float currentLifePoint;
    [HideInInspector] public float maxLifePoint;
    [HideInInspector] public bool isInvincible;
    [HideInInspector] public bool haveGloves;

    [Header("Movements Stats")]
    public float moveSpeed;
    public float rotationSpeed;
    public float rollCooldown;
    public AnimationCurve rollAnimationCurve;
    public float grappleFlySpeed;
    
    [Header("Attack Stats")]

    // Melee Attack
    public int attackDamage;
    public float attackSpeed;
    
    // Distance Attack
    public GameObject throwingWeapon;
    public float throwingSpeed;
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public string state = "StatusQuo";
    
    //Others
    [Header("Others")] 
    public AnimationClip rollAnimClip;
    [HideInInspector] public bool inputInteractPushed;
    [HideInInspector] public Vector3 move; //For Arcade Easter Egg
    [SerializeField] private Transform moveDirection;

    #endregion
    
    #region Private variables

    //Movement
    private float m_speed;
    private Vector3 m_moveDirection;
    private bool m_moving;
    private Quaternion m_lookRot;
    
    //Roll
    private bool m_canRoll;
    private bool m_isRolling;
    private float m_acTimer; //animated curve current timer
    private bool RollCdStarted;
    private float m_cdRoll;

    private bool isDead;


    //Animations
    private static readonly int AttackSpeed = Animator.StringToHash("AttackSpeed");
    private static readonly int Moving = Animator.StringToHash("Moving");

    #endregion
    
    private void Awake()
    {
        #region Singleton

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        #endregion
        
        m_inputController = new InputController();
        
        m_animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rope = GetComponent<Rope>();
        m_playerInput = GetComponent<PlayerInput>();
        m_attack = GetComponentInChildren<Attack>();
    }

    private void Start()
    {
        m_playerInput.actions = m_inputController.asset;
        
        //Stats Variables Setup
        currentLifePoint = maxLifePoint = baseLifePoint;
        m_speed = moveSpeed;
        m_canRoll = true;
        m_isRolling = false;

        //Roll Animation Curve Setup
        rollAnimationCurve.keys[rollAnimationCurve.length -1].time = rollAnimClip.length;
        m_acTimer = rollAnimationCurve.keys[rollAnimationCurve.length -1].time;
    }
    
    private void Update()
    {
        Cursor.visible = m_playerInput.currentControlScheme == "Keyboard&Mouse"; //add a locker
        
        //Read Input
        
        //Interact
        if (!isDead)
        {
            m_inputController.Player.Interact.started += Interact;
            m_inputController.Player.Interact.canceled += Interact;
        
            //Clamp Rope
            m_inputController.Player.Clamp.started += _ => rope.isClamped = !rope.isClamped;
        
            //Roll
            m_inputController.Player.Roll.started += Roll;

            if (m_isRolling)
            {
                m_canRoll = false;

                m_acTimer -= Time.deltaTime;
            
                m_speed = rollAnimationCurve.Evaluate(m_acTimer);
            
                Debug.Log(m_speed);
            
                if (m_acTimer <= 0)
                {
                    m_isRolling = false;
                }
            
            }
            else
            {
                m_speed = moveSpeed;
                m_acTimer = rollAnimationCurve.keys[rollAnimationCurve.length -1].time;
            }
        
            if (!m_canRoll)
            {
                if (!RollCdStarted)
                {
                    RollCdStarted = true;
                    m_cdRoll = rollCooldown;
                }
                else
                {
                    m_cdRoll -= Time.deltaTime;
                
                    if (m_cdRoll <= 0)
                    {
                        m_canRoll = true;
                    }
                }
            
            }
            else
            {
                RollCdStarted = false;
            }
        
            //Move
            m_inputController.Player.Move.started += Move;
            m_inputController.Player.Move.performed += Move;
            m_inputController.Player.Move.canceled += Move;
            if (m_moving && !m_attack.isAttacking) rb.velocity = !m_attack.isAttacking ? Quaternion.Euler(0,-45,0) * new Vector3(m_moveDirection.x * m_speed, rb.velocity.y, m_moveDirection.z * m_speed ) : Vector3.zero;

        
            //Attack Melee
            m_inputController.Player.Melee.started += Attack;
        
            //Distance
            m_inputController.Player.Range.started += Range;


            if (!m_attack.isAttacking)
            {
                switch (state)
                {
                    case "StatusQuo":
                        m_inputController.Player.Range.started += _ => Throw();
                        break;
                    case "Rope":
                        m_inputController.Player.Range.started += _ => Rewind();
                        break;
                    case "Throw":
                        throwingWeapon.transform.Translate(direction * (Time.deltaTime * throwingSpeed));
                        break;
                    default: return;
                } 
            }
            
            
        }

        m_inputController.Player.MousePosition.performed += context => m_mousePos = context.ReadValue<Vector2>();
                
        
        m_inputController.Player.Bugtracker.started += _ => GameManager.instance.OpenBugTrackerPanel(!GameManager.instance.bugtracker.reportPanel.activeSelf);
        m_inputController.Player.Bugtracker.started += _ => m_controlState = GameManager.instance.bugtracker.reportPanel.activeSelf ? ControlState.UI : ControlState.NORMAL;
        m_inputController.Player.Bugtracker.started += _ => Time.timeScale = GameManager.instance.bugtracker.reportPanel.activeSelf ? 0 : 1;
        
        m_inputController.Player.Pause.started += PauseOnStarted;
    }

    private void Range(InputAction.CallbackContext range)
    {

    }

    private void Attack(InputAction.CallbackContext attack)
    {
        if (attack.started)
        {
            if (!m_isRolling && !m_attack.isAttacking)
            {
                m_canRoll = false;
                m_attack.isAttacking = true;
                m_attack.canHurt = true;

                m_animator.SetFloat(AttackSpeed, attackSpeed);

                m_animator.Play("Attack");
            }
            
        }
    }

    private void Move(InputAction.CallbackContext moveInput)
    {
        m_moveDirection = new Vector3(moveInput.ReadValue<Vector2>().x, 0, moveInput.ReadValue<Vector2>().y);
        move = new Vector3(moveInput.ReadValue<Vector2>().x, 0, moveInput.ReadValue<Vector2>().y);

        if (moveInput.performed)
        {
            
                if (!m_isRolling && !m_attack.isAttacking)
                {
                    m_animator.Play("Move");
                }
            
                m_moving = true;

                //Rotation
            Quaternion lookRotation = Quaternion.LookRotation(Quaternion.Euler(0,-45,0) * m_moveDirection);
            rb.MoveRotation(lookRotation);
            
            
            // m_lookRot = Quaternion.LookRotation(m_moveDirection);
            // rb.DORotate(m_lookRot.eulerAngles, rotationSpeed);
            
            //FX
            
            //Instantiate(Poufpouf, transform.position, Quaternion.identity);
        }

        if (moveInput.canceled)
        {
            if (!m_attack.isAttacking && !m_isRolling)
            {
                m_animator.Play("Idle");
                
                m_moving = false;
                rb.velocity = Vector3.zero;
            }
        }
    }

    private void Roll(InputAction.CallbackContext roll)
    {
        if (roll.started)
        {
            if (!m_attack.isAttacking)
            {
                if (m_canRoll)
                {
                    m_animator.Play("Roll");
                    m_isRolling = true;

                }

            }
        }
    }
    
    private void Interact(InputAction.CallbackContext interact)
    {
        if (interact.started)
        {
            inputInteractPushed = true;
        }

        if (interact.canceled)
        {
            inputInteractPushed = false;
        }
    }

    public IEnumerator StartStun(float stunDuration)
    {
        m_animator.Play("Electrocut");
        m_playerStateMachine = PlayerStateMachine.STUN;
        yield return new WaitForSeconds(stunDuration);
        m_playerStateMachine = PlayerStateMachine.IDLE;
    }
    
    private void PauseOnStarted(InputAction.CallbackContext obj)
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

    private void Rotation()
    {
        if (m_playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            switch (m_attack.isAttacking)
            {
                case true :
                    if (Camera.main)
                    {
                        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(m_mousePos);
                        Vector3 aimDirection = (mousePosition - transform.position).normalized;
                
                        m_lookRot = Quaternion.LookRotation(new Vector3(aimDirection.x, 0, aimDirection.z));
                        rb.DORotate(m_lookRot.eulerAngles, 0);
                    }
                    break;
            }
        }
    }


    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    private void Throw()
    {
        if(state == "StatusQuo")
        {
            m_animator.Play("Throw");
            throwingWeapon.SetActive(true);
            throwingWeapon.transform.position = transform.position + transform.forward * 0.5f;
            throwingWeapon.transform.LookAt(throwingWeapon.transform.position+transform.forward);
            
            direction = Vector3.forward;
            
            state = "Throw";
            rope.enabled = true;
            rope.rope.gameObject.SetActive(true);
        }
    }
    
    public void Rewind()
    {
        if (state == "Rope")
        {
            rope.rewinding = true;
        }
    }

    public void GetHurt(int damage)
    {
        if (isInvincible) return;
        
        if (currentLifePoint > 0)
        {
            currentLifePoint -= damage;
            
            if (!m_attack.isAttacking)
            {
                m_animator.Play("Hurt");
            }

            if (currentLifePoint <= 0)
            {
                m_animator.Play("Death");
                isDead = true;
                m_playerStateMachine = PlayerStateMachine.DEAD;
                
            }
        }
        
    }

    public void RespawnPlayer()
    {
        GameManager.instance.BackToCheckpoint();
        ResetState();
        isDead = false;
        currentLifePoint = maxLifePoint;
    }
    
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public void ResetState()
    {
        m_isRolling = false;
        m_attack.isAttacking = false;

        if (m_moving)
        {
            m_animator.Play("Move");
        }
        else
        {
            m_animator.Play("Idle");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Item item = other.GetComponent<Item>();
        
        if (item)
        {
            switch (item.affectedValue)
                {
                    case "Health":
                        currentLifePoint +=  item.valuePercentage;
                        Debug.Log("Got healed by " + item.valuePercentage);
                        break;
                
                    case "Rope":
                        rope.maximumLenght += item.valuePercentage;
                        break;
                }
            
            Destroy(item.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Grappin>() && inputInteractPushed && rope.maximumLenght >= other.GetComponent<Grappin>().ropeSizeNecessary)
        {
            transform.DOMove(other.GetComponent<Grappin>().pointToGo.position, grappleFlySpeed);
        }
    }

    public void OnRespawn()
    {
        transform.position = GameManager.instance.lastCheckpoint.respawnPoint.position;
    }

    private void OnEnable()
    {
        m_inputController.Enable();
    }

    private void OnDisable()
    {
        m_inputController.Disable();
    }

    public void EnterDialogue()
    {
        m_controlState = ControlState.DIALOGUE;
    }
    
    public void ExitDialogue()
    {
        m_controlState = ControlState.NORMAL;
    }
}
