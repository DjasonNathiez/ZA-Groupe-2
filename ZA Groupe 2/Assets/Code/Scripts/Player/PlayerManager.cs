using System.Collections;
using System.Diagnostics.CodeAnalysis;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[SuppressMessage("ReSharper", "CheckNamespace")]
public class PlayerManager : MonoBehaviour
{
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
    [HideInInspector] public TestRope rope;

    #endregion

    #region Player States

    private ControlState m_controlState;

    private enum ControlState
    {
        NORMAL,
        UI
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
    [HideInInspector] public bool isStun;

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
        rope = GetComponent<TestRope>();
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
        
        CheckForAnimation();

        if (!m_attack.isAttacking && m_playerStateMachine is not (PlayerStateMachine.DEAD or PlayerStateMachine.STUN))
        {
            m_playerStateMachine = m_moveDirection != Vector3.zero ? PlayerStateMachine.MOVE : PlayerStateMachine.IDLE;
        }
        
        //Read Input
        switch (m_controlState)
        {
            case ControlState.NORMAL:

                switch (m_playerStateMachine)
                {
                    case PlayerStateMachine.IDLE:
                        
                        m_inputController.Player.Melee.started += _ => LoadAttack();
                        
                        m_inputController.Player.Interact.started += _ => inputInteractPushed = true;
                        m_inputController.Player.Interact.canceled += _ => inputInteractPushed = false;
                        
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
                        
                        if (m_canRoll)
                        {
                            m_inputController.Player.Roll.started += _ => m_isRolling = true;
                            m_inputController.Player.Roll.started += _ => m_animator.Play("Roll");
                        }
                        
                        m_inputController.Player.Move.performed += context => m_moveDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
                        m_inputController.Player.Move.performed += context => move = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);

                        m_inputController.Player.Move.performed += _ => m_moving = true;
                        
                        m_inputController.Player.Move.canceled += _ => m_moveDirection = Vector3.zero;
                        m_inputController.Player.Move.canceled += _ => m_moving = false;
                        
                        
                        Move();
                        Rotation(); 
                        
                        break;
                    
                    case PlayerStateMachine.MOVE:
                        
                        m_inputController.Player.Melee.started += _ => LoadAttack();
                        
                        m_inputController.Player.Interact.started += _ => inputInteractPushed = true;
                        m_inputController.Player.Interact.canceled += _ => inputInteractPushed = false;
                        
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
                        
                        if (m_canRoll)
                        {
                            m_inputController.Player.Roll.started += _ => m_isRolling = true;
                            m_inputController.Player.Roll.started += _ => m_animator.Play("Roll");
                        }
                        
                        m_inputController.Player.Move.performed += context => m_moveDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
                        m_inputController.Player.Move.performed += context => move = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);

                        m_inputController.Player.Move.performed += _ => m_moving = true;
                        
                        m_inputController.Player.Move.canceled += _ => m_moveDirection = Vector3.zero;
                        m_inputController.Player.Move.canceled += _ => m_moving = false;
                        
                        
                        Move();
                        Rotation();
                        
                        break;

                    case PlayerStateMachine.STUN:
                        m_moving = false;
                        move = Vector3.zero;
                        m_moveDirection = Vector3.zero;
                        rb.velocity = Vector3.zero;
                        break;
                    
                    case PlayerStateMachine.DEAD:
                        m_moving = false;
                        m_moveDirection = Vector3.zero;
                        move = Vector3.zero;
                        rb.velocity = Vector3.zero;
                        break;
                }

                m_inputController.Player.MousePosition.performed += context => m_mousePos = context.ReadValue<Vector2>();
 
                if (m_isRolling)
                {
                    StartCoroutine(StartRoll());
                }
                
                break;
            
            case ControlState.UI:
                
                //can't access player base controls
                break;
        }
        
        m_inputController.Player.Bugtracker.started += _ => GameManager.instance.OpenBugTrackerPanel(!GameManager.instance.bugtracker.reportPanel.activeSelf);
        m_inputController.Player.Bugtracker.started += _ => m_controlState = GameManager.instance.bugtracker.reportPanel.activeSelf ? ControlState.UI : ControlState.NORMAL;
        m_inputController.Player.Bugtracker.started += _ => Time.timeScale = GameManager.instance.bugtracker.reportPanel.activeSelf ? 0 : 1;
        
        m_inputController.Player.Pause.started += PauseOnStarted;
    }

    private void CheckForAnimation()
    {
        m_animator.SetBool(Moving, m_moving);
        
        switch (m_playerStateMachine)
        {
            case PlayerStateMachine.IDLE:
                m_animator.Play("Idle");
                break;
            
            case PlayerStateMachine.MOVE:

                if (!m_attack.isAttacking)
                {
                    m_animator.Play("Move");
                }
                break;
            
            case PlayerStateMachine.ROLLING:
                break;
            
            case PlayerStateMachine.THROW:
                m_animator.Play("Throw");
                break;
            
            case PlayerStateMachine.DEAD:
                m_animator.Play("Death");
                break;
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
    
    private void Move()
    {
        rb.velocity = !m_attack.isAttacking ? new Vector3(m_moveDirection.x * m_speed, rb.velocity.y, m_moveDirection.z * m_speed ) : Vector3.zero;
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
                
                case false :
                    if (m_moveDirection != Vector3.zero)
                    {
                        m_lookRot = Quaternion.LookRotation(m_moveDirection);
                        rb.DORotate(m_lookRot.eulerAngles, rotationSpeed);
                    }
                    break;
            }
        }
        else
        {
            if (m_moveDirection != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(m_moveDirection);
                rb.MoveRotation(lookRotation);
            }
        }
    }
    
    IEnumerator StartRoll()
    {
        while (true)
        {
            m_canRoll = false;
        
            m_playerStateMachine = PlayerStateMachine.ROLLING;
        
            m_acTimer -= Time.deltaTime;
            m_speed = rollAnimationCurve.Evaluate(m_acTimer);

            yield return new WaitUntil(() => m_acTimer <= 0);
        
            m_speed = moveSpeed;
            m_isRolling = false;
            m_acTimer = rollAnimationCurve.keys[rollAnimationCurve.length -1].time;
                
            m_playerStateMachine = !m_moving ? PlayerStateMachine.IDLE : PlayerStateMachine.MOVE;
            
            yield return new WaitForSeconds(rollCooldown);
        
            m_canRoll = true;
            break;
        }

    }
    
    private void LoadAttack()
    {
        m_playerStateMachine = PlayerStateMachine.ATTACK;
        m_attack.isAttacking = true;
        
        m_animator.SetFloat(AttackSpeed, attackSpeed);
        m_animator.Play("Attack");
    }
    
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
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
            m_animator.Play("Hurt");

            if (currentLifePoint <= 0)
            {
                m_animator.Play("Death");
                m_playerStateMachine = PlayerStateMachine.DEAD;
            }
        }
        
    }

    public void RespawnPlayer()
    {
        GameManager.instance.BackToCheckpoint();
        ResetState();
        currentLifePoint = maxLifePoint;
    }
    
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public void ResetState()
    {
        m_attack.isAttacking = false;
        Debug.Log("Reset State");
        m_playerStateMachine = !m_moving ? PlayerStateMachine.IDLE : PlayerStateMachine.MOVE;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Grappin>() && inputInteractPushed)
        {
            transform.DOMove(other.GetComponent<Grappin>().pointToGo.position, grappleFlySpeed);
        }
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
