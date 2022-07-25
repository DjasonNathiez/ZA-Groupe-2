using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[SuppressMessage("ReSharper", "CheckNamespace")]
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance; //Singleton

    public ParticleSystem VFXPoufpouf;

    #region Components

    //Unity components 
    private InputController m_inputController;
    [HideInInspector] public Rigidbody rb;
    private Animator m_animator;
    private PlayerInput m_playerInput;
    private Vector2 m_mousePos;

    //Personal scripts components
    private Attack m_attack;
    public Rope rope;

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

    [Header("States Stats")] public float baseLifePoint;
    public float currentLifePoint;
    [HideInInspector] public float maxLifePoint;
    [HideInInspector] public bool isInvincible;
    [HideInInspector] public bool haveGloves;
    public float invicibiltyTimer;

    public bool isAttacking;
    public bool isMoving;
    public bool isRolling;
    public bool collect;
    public bool isThrowing;
    public bool isElectrocut;
    public bool isHurt;
    public bool startTalking;
    public bool isTalking;
    public int storyState;
    public bool isTeleporting;

    [Header("Movements Stats")] public float moveSpeed;
    public float rotationSpeed;
    public float rollCooldown;
    public AnimationCurve rollAnimationCurve;
    public float grappleFlySpeed;
    public bool buttonAPressed;

    [Header("Attack Stats")]

    // Melee Attack
    public int attackDamage;

    public float attackSpeed;

    // Distance Attack
    public GameObject throwingWeapon;
    public float throwingSpeed;
    [HideInInspector] public Vector3 direction;
    public ActionType state = ActionType.StatusQuo;
    public GameObject playerThrowingWeapon;
    public float aimHelpAngle;

    //Others
    [Header("Others")] public AnimationClip rollAnimClip;
    public AnimationClip attackAnimClip;
    [HideInInspector] public bool inputInteractPushed;
    [HideInInspector] public Vector3 move; //For Arcade Easter Egg
    [SerializeField] private Transform moveDirection;
    public GameObject dialogueBox;
    public TextEffectManager textEffectManager;
    public CameraController cameraController;
    public GameObject manoirLight;
    public LineRenderer ropeGrappin;

    [Header("Debug Commands")] public GameObject godModeText;
    public GameObject speedBoostText;
    public GameObject maxLevelText;

    //Inventory
    [Header("Inventory")] public bool gloves;
    public string startHat;
    public GameObject currentHat;
    public MeshFilter hatMesh;
    public MeshRenderer HatMeshRenderer;
    public Hat[] hats;

    [Serializable]
    public class Hat
    {
        public string hatName;
        public string displayName;
        public string description;
        public bool collected;
        public GameObject hatObj;
        public Material baseMaterial;
    }

    #endregion
    
    #region Private variables

    //Movement
    private float m_speed;
    private bool isSpeedBoost;
    private Vector3 m_moveDirection;
    private bool m_moving;
    private Quaternion m_lookRot;

    [SerializeField] private float poufpoufTime;

    private float poufpoufTimer;

    [SerializeField] public Vector3 poufpoufOffset;
    public bool poufpoufInstantiated;

    //Roll
    private bool m_canRoll;
    private bool m_isRolling;
    private float m_acTimer; //animated curve current timer
    private bool RollCdStarted;
    private float m_cdRoll;

    public bool isDead;
    [SerializeField] private CapsuleCollider normalCollider;
    [SerializeField] private CapsuleCollider rollCollider;
    [SerializeField] private Vector3 rollCenter;
    [SerializeField] private float rollHeight;
    [SerializeField] private Vector3 normalCenter;
    [SerializeField] private float normalHeight;


    //Animations
    private static readonly int AttackSpeed = Animator.StringToHash("AttackSpeed");
    private static readonly int Moving = Animator.StringToHash("Moving");
    private float attackCD;
    private float currentAttackCD;

    //Attacks
    [SerializeField] private GameObject visuthrow;

    #endregion

    #region VFX

    [Header("VFX")] public ParticleSystem attackVFX;
    public ParticleSystem hurtVFX;
    public ParticleSystem rollVFX;

    public ParticleSystem throwingVFX;
    public GameObject heavyVFX;
    public ParticleSystem throwHit;
    public ParticleSystem throwHitEnemy;

    public ParticleSystem collectVFX;
    public ParticleSystem kentScaredVFX;

    #endregion

    #region HurtImpact

    [Header("Visual Hurt")] public Material modelKent;
    public AnimationCurve animationHurt;
    public float hurtTime;
    public bool hurtAnim;
    [Header("Visual Blink")] public AnimationCurve blinkIteration;
    public AnimationCurve blinkSpeed;
    public float blinkTime;
    public bool blinkAnim;

    [SerializeField] private CameraShakeScriptable gethurtShakePos;
    [SerializeField] private CameraShakeScriptable gethurtShakeRot;
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


        foreach (Hat i in hats)
        {
            if (i.hatName == startHat)
            {
                currentHat = i.hatObj;
                i.hatObj.SetActive(true);
            }
            else
            {
                i.hatObj.SetActive(false);
            }
        }

        m_inputController = new InputController();

        m_animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rope = GetComponent<Rope>();
        m_playerInput = GetComponent<PlayerInput>();
        m_attack = GetComponentInChildren<Attack>();
    }


    public void SwitchHat(string selectedHat)
    {
        foreach (Hat hat in hats)
        {
            if (hat.hatName == selectedHat)
            {
                currentHat = hat.hatObj;
                hat.hatObj.SetActive(true);
            }
            else
            {
                hat.hatObj.SetActive(false);
            }
        }
    }

    private void Start()
    {
        m_playerInput.actions = m_inputController.asset;
        poufpoufTimer = poufpoufTime;
        //Stats Variables Setup
        currentLifePoint = maxLifePoint = baseLifePoint;
        m_speed = moveSpeed;
        m_canRoll = true;
        m_isRolling = false;
        heavyVFX.SetActive(false);
        attackCD = attackAnimClip.length;

        //Roll Animation Curve Setup
        rollAnimationCurve.keys[rollAnimationCurve.length - 1].time = rollAnimClip.length;
        m_acTimer = rollAnimationCurve.keys[rollAnimationCurve.length - 1].time;

        if (GameManager.instance) GameManager.instance.ui.UpdateHat();
    }
    
    private void Update()
    {
        if (storyState == 1 && !GameManager.instance.dungeonEnded)
        {
            GameManager.instance.dungeonEnded = true;
        }

        if (currentLifePoint > maxLifePoint)
        {
            currentLifePoint = maxLifePoint;
        }

        //animator Set Bool
        m_animator.SetBool("isAttacking", isAttacking);
        m_animator.SetBool("isMoving", isMoving);
        m_animator.SetBool("isRolling", isRolling);
        m_animator.SetBool("collect", collect);
        m_animator.SetBool("isThrowing", isThrowing);
        m_animator.SetBool("isElectrocut", isElectrocut);
        m_animator.SetBool("isHurt", isHurt);
        m_animator.SetBool("startTalking", startTalking);
        m_animator.SetBool("isTalking", isTalking);
        m_animator.SetBool("isDead", isDead);

        if (isRolling)
        {
            isAttacking = false;
        }

        if (isAttacking)
        {
            isRolling = false;
            normalCollider.center = normalCenter;
            normalCollider.height = normalHeight;
        }

        if (hurtAnim)
        {
            modelKent.SetFloat("_UseOnEmission", animationHurt.Evaluate(Time.time - hurtTime));
            modelKent.SetFloat("_UseOnAlbedo", animationHurt.Evaluate(Time.time - hurtTime));
            if (Time.time - hurtTime > animationHurt.keys[animationHurt.keys.Length - 1].time) hurtAnim = false;
        }

        if (blinkAnim)
        {
            modelKent.SetFloat("_UseInvincibility", 1);
            modelKent.SetFloat("_Speed_B", blinkSpeed.Evaluate(Time.time - blinkTime));
            modelKent.SetFloat("_Iteration_B", blinkIteration.Evaluate(Time.time - blinkTime));
            if (Time.time - blinkTime > blinkSpeed.keys[animationHurt.keys.Length - 1].time)
            {
                blinkAnim = false;
                modelKent.SetFloat("_UseInvincibility", 0);
            }
        }

        #region Read Input

        if (!isDead && m_controlState != ControlState.DIALOGUE)
        {
            m_inputController.Player.Interact.started += Interact;
            m_inputController.Player.Interact.canceled += Interact;

            //Roll
            if (!isRolling && !isAttacking && isMoving && m_controlState != ControlState.DIALOGUE)
            {
                m_inputController.Player.Roll.started += Roll;
            }

            m_inputController.Player.Roll.started += RollInput;
            m_inputController.Player.Roll.canceled += RollInput;
            if (m_isRolling)
            {
                m_canRoll = false;

                m_acTimer -= Time.deltaTime;

                m_speed = rollAnimationCurve.Evaluate(m_acTimer);

                if (m_acTimer <= 0)
                {
                    m_isRolling = false;
                    normalCollider.center = normalCenter;
                    normalCollider.height = normalHeight;
                }
            }
            else
            {
                m_speed = moveSpeed;
                m_acTimer = rollAnimationCurve.keys[rollAnimationCurve.length - 1].time;
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
            if (m_controlState != ControlState.DIALOGUE)
            {
                m_inputController.Player.Move.started += Move;
                m_inputController.Player.Move.performed += Move;
                m_inputController.Player.Move.canceled += Move;
            }


            if (!m_attack.isAttacking && isMoving)
            {
                if (state != ActionType.Aiming && state != ActionType.Throwing)
                    rb.velocity = Quaternion.Euler(0, cameraController.transform.eulerAngles.y, 0) * new Vector3(m_moveDirection.x * m_speed, rb.velocity.y,
                        m_moveDirection.z * m_speed);
            }
            else
            {
                rb.velocity = Vector3.zero;
            }


            //Attack Melee
            if (!isAttacking && !isRolling)
            {
                m_inputController.Player.Melee.started += Attack;
            }

            if (currentAttackCD > 0)
            {
                currentAttackCD -= Time.deltaTime;
            }
            else if (currentAttackCD <= 0)
            {
                m_attack.isAttacking = false;
            }

            //Distance
            if (state == ActionType.Aiming)
            {
                if (m_inputController.Player.Move.ReadValue<Vector2>() != Vector2.zero)
                {
                    List<ValueTrack> reachable = new List<ValueTrack>(0);

                    foreach (ValueTrack obj in GameManager.instance.grippableObj)
                    {
                        if (obj == null || obj.GetComponent<ValueTrack>() == null) return;
                        if (Vector2.SqrMagnitude(new Vector2(obj.transform.position.x, obj.transform.position.z) -
                                                 new Vector2(transform.position.x, transform.position.z)) <
                            Mathf.Clamp(rope.maximumLenght, 0, 12) * Mathf.Clamp(rope.maximumLenght, 0, 12) &&
                            obj.transform.position.y > transform.position.y - 1 &&
                            obj.transform.position.y < transform.position.y + 1)
                        {
                            reachable.Add(obj);
                        }
                    }

                    ValueTrack nearest = null;
                    float angle = aimHelpAngle;
                    Vector2 test = Quaternion.Euler(0, 0, -cameraController.transform.eulerAngles.y) *
                                   m_inputController.Player.Move.ReadValue<Vector2>();
                    foreach (ValueTrack obj in reachable)
                    {
                        if (Vector2.Angle(test,
                                new Vector2(obj.transform.position.x, obj.transform.position.z) -
                                new Vector2(transform.position.x, transform.position.z)) < angle)
                        {
                            angle = Vector2.Angle(test,
                                new Vector2(obj.transform.position.x, obj.transform.position.z) -
                                new Vector2(transform.position.x, transform.position.z));
                            nearest = obj;
                        }
                    }

                    if (nearest != null)
                    {
                        transform.rotation = Quaternion.LookRotation(
                            new Vector3(nearest.transform.position.x, 0, nearest.transform.position.z) -
                            new Vector3(transform.position.x, 0, transform.position.z));
                        foreach (ValueTrack obj in reachable)
                        {
                            if (nearest == obj && obj.meshRenderer != null)
                            {
                                if (obj.isEnemy)
                                {
                                    obj.meshRenderer.material.SetFloat("_EnableOutline", 1);
                                    obj.gameObject.GetComponent<AIBrain>().modelAggroMat.SetFloat("_EnableOutline", 1);
                                    obj.gameObject.GetComponent<AIBrain>().modelNonAggroMat
                                        .SetFloat("_EnableOutline", 1);
                                }
                                else obj.meshRenderer.material.SetFloat("_EnableOutline", 1);
                            }
                            else if (nearest != obj && obj.meshRenderer != null)
                            {
                                obj.meshRenderer.material.SetFloat("_EnableOutline", 0);

                                if (!obj.isEnemy) continue;
                                if (obj == null) return;
                                obj.gameObject.GetComponent<AIBrain>().modelAggroMat.SetFloat("_EnableOutline", 0);
                                obj.gameObject.GetComponent<AIBrain>().modelNonAggroMat.SetFloat("_EnableOutline", 0);
                            }
                        }
                    }
                    else
                    {
                        foreach (ValueTrack obj in reachable)
                        {
                            if (obj.meshRenderer != null)
                            {
                                obj.meshRenderer.material.SetFloat("_EnableOutline", 0);

                                if (!obj.isEnemy) continue;
                                if (obj == null) return;
                                obj.gameObject.GetComponent<AIBrain>().modelAggroMat.SetFloat("_EnableOutline", 0);
                                obj.gameObject.GetComponent<AIBrain>().modelNonAggroMat.SetFloat("_EnableOutline", 0);
                            }
                        }
                    }
                }
                else
                {
                    foreach (ValueTrack obj in GameManager.instance.grippableObj)
                    {
                        if (obj.meshRenderer != null) obj.meshRenderer.material.SetFloat("_EnableOutline", 0);

                        if (!obj.isEnemy) continue;
                        if (obj == null) return;
                        obj.gameObject.GetComponent<AIBrain>().modelAggroMat.SetFloat("_EnableOutline", 0);
                        obj.gameObject.GetComponent<AIBrain>().modelNonAggroMat.SetFloat("_EnableOutline", 0);
                    }
                }
            }

            if (state != ActionType.Aiming)
            {
                foreach (ValueTrack obj in GameManager.instance.grippableObj)
                {
                    if (obj.meshRenderer != null)
                    {
                        obj.meshRenderer.material.SetFloat("_EnableOutline", 0);
                    }

                    if (!obj.isEnemy) continue;
                    if (obj == null) return;
                    obj.gameObject.GetComponent<AIBrain>().modelAggroMat.SetFloat("_EnableOutline", 0);
                    obj.gameObject.GetComponent<AIBrain>().modelNonAggroMat.SetFloat("_EnableOutline", 0);
                }
            }
        }

        m_inputController.Player.MousePosition.performed += context => m_mousePos = context.ReadValue<Vector2>();


        m_inputController.Player.Bugtracker.started += _ =>
            GameManager.instance.OpenBugTrackerPanel(!GameManager.instance.bugtracker.reportPanel.activeSelf);
        m_inputController.Player.Bugtracker.started += _ =>
            m_controlState = GameManager.instance.bugtracker.reportPanel.activeSelf
                ? ControlState.UI
                : ControlState.NORMAL;
        m_inputController.Player.Bugtracker.started += _ =>
            Time.timeScale = GameManager.instance.bugtracker.reportPanel.activeSelf ? 0 : 1;


        m_inputController.Player.Pause.started += PauseOnStarted;

        #endregion
        
        // Interface

        if (state != ActionType.StatusQuo)
        {
            if(!GameManager.instance.ui.ropeSlider.gameObject.activeSelf) GameManager.instance.ui.ropeSlider.gameObject.SetActive(true);
            GameManager.instance.ui.UpdateRopeLenght();
        }
        else
        {
            GameManager.instance.ui.ropeSlider.value = 0;
            if (GameManager.instance.ui.ropeSlider.gameObject.activeSelf) GameManager.instance.ui.ropeSlider.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (state == ActionType.Throwing)
        {
            throwingWeapon.transform.Translate(direction * (Time.fixedDeltaTime * throwingSpeed));
        }
    }

    #region ATTACK

    void SetAttackCD()
    {
        currentAttackCD = attackCD;
    }

    private void Attack(InputAction.CallbackContext attack)
    {
        if (attack.started)
        {
            if (!m_isRolling && !m_attack.isAttacking && !isDead && !isAttacking &&
                (state is ActionType.StatusQuo or ActionType.RopeAttached) &&
                m_controlState != ControlState.DIALOGUE)
            {
                if (state == ActionType.RopeAttached) Rewind();

                m_canRoll = false;
                m_attack.isAttacking = true;
                m_attack.m_collider.enabled = true;
                m_attack.canHurt = true;
                
                SetAttackCD();

                m_animator.SetFloat(AttackSpeed, attackSpeed);
                attackVFX.Play();
                PlaySFX("P_Attack");
                isAttacking = true;
            }
        }
    }

    #endregion

    #region DISTANCE

    private void Throw()
    {
        if (isTeleporting) return;
        
        if (m_controlState != ControlState.DIALOGUE)
        {
            if (state == ActionType.Aiming)
            {
                isThrowing = true;

                throwingWeapon.SetActive(true);
                throwingWeapon.transform.position = transform.position + transform.forward * 0.5f;
                throwingWeapon.transform.LookAt(throwingWeapon.transform.position + transform.forward);

                direction = Vector3.forward;
                state = ActionType.Throwing;
                playerThrowingWeapon.SetActive(false);
                rope.enabled = true;
                rope.rope.gameObject.SetActive(true);
                PlaySFX("P_Throw");
                PlaySFX("P_RopeThrow");
            }
        }
    }

    public void Rewind()
    {
        if (m_controlState != ControlState.DIALOGUE)
        {
            if (state is ActionType.RopeAttached or ActionType.Throwing)
            {
                isThrowing = false;
                rope.rewinding = true;
                PlaySFX("P_Rewind");
            }
        }
    }

    public void OnRange()
    {
        if (isTeleporting) return;

        if (m_controlState == ControlState.DIALOGUE) return;

        if (!isDead)
        {
            switch (state)
            {
                case ActionType.StatusQuo:
                    if (!m_attack.isAttacking && !m_isRolling)
                    {
                        rb.velocity = Vector3.zero;
                        ClearAimList();
                        state = ActionType.Aiming;
                        visuthrow.SetActive(true);
                    }

                    break;
                case ActionType.Throwing:
                    Rewind();
                    break;
                case ActionType.RopeAttached:
                    Rewind();
                    break;
                default: return;
            }
        }
    }

    public void OnOutRange()
    {
        if (!isDead)
        {
            switch (state)
            {
                case ActionType.Aiming:
                    Throw();
                    visuthrow.SetActive(false);
                    ClearAimList();
                    break;
                default: return;
            }
        }
    }

    #endregion

    #region MOVEMENT

    private void Move(InputAction.CallbackContext moveInput)
    {
        if (isTeleporting)
        {
            isMoving = false;
            rb.velocity = Vector3.zero;
            return;
        }

        m_moveDirection = new Vector3(moveInput.ReadValue<Vector2>().x, 0, moveInput.ReadValue<Vector2>().y);
        move = new Vector3(moveInput.ReadValue<Vector2>().x, 0, moveInput.ReadValue<Vector2>().y);

        if (moveInput.performed)
        {
            if (m_moveDirection.magnitude > 0.2f && m_controlState != ControlState.DIALOGUE)
            {
                if (!m_isRolling && !m_attack.isAttacking && !isDead)
                {
                    isMoving = true;

                    //Rotation
                    if (!isDead && m_controlState != ControlState.DIALOGUE)
                    {
                        Quaternion lookRotation =
                            Quaternion.LookRotation(Quaternion.Euler(0, cameraController.transform.eulerAngles.y, 0) * m_moveDirection);
                        rb.MoveRotation(lookRotation);
                    }
                }
            }
            else
            {
                isMoving = false;
            }

            m_moving = true;
        }

        if (moveInput.canceled)
        {
            isMoving = false;

            if (!m_attack.isAttacking && !m_isRolling && !isDead)
            {
                poufpoufTimer = poufpoufTime;
                m_moving = false;
                rb.velocity = Vector3.zero;
            }
        }
    }
    
    private void Roll(InputAction.CallbackContext roll)
    {
        if (isTeleporting) return;

        if (roll.started)
        {
            if (!m_attack.isAttacking && !isDead && isMoving)
            {
                if (m_canRoll)
                {
                    rollVFX.Play();
                    PlaySFX("P_Roll");
                    isRolling = true;
                    normalCollider.center = rollCenter;
                    normalCollider.height = rollHeight;
                    m_isRolling = true;
                }
            }
        }
    }


    private void Rotation()
    {
        if (m_playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            switch (m_attack.isAttacking)
            {
                case true:
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
    

    #endregion

    #region STATE

    public void GetHurt(int damage)
    {
        if (!isDead)
        {
            if (isInvincible) return;

            if (currentLifePoint > 0)
            {
                currentLifePoint -= damage;
                StartCoroutine(InvincibilityFrame());
                PlaySFX("P_Hurt");
                hurtVFX.Play();
                
                CameraShake.Instance.AddShakeEvent(gethurtShakePos);
                CameraShake.Instance.AddShakeEvent(gethurtShakeRot);
                
                hurtAnim = true;
                hurtTime = Time.time;
                blinkTime = Time.time;
                GameManager.instance.ui.UpdateHealth();
                if (!m_attack.isAttacking)
                {
                    isHurt = true;
                }
            }

            if (currentLifePoint <= 0)
            {
                GameManager.instance.DisableAllEnemy();
                StartCoroutine(WaitForRespawn());
                isDead = true;
                m_playerStateMachine = PlayerStateMachine.DEAD;
            }
        }
    }

    public IEnumerator InvincibilityFrame()
    {
        isInvincible = true;
        blinkAnim = true;
        yield return new WaitForSeconds(invicibiltyTimer);
        isInvincible = false;
    }

    public IEnumerator StartStun(float stunDuration)
    {
        isElectrocut = true;
        m_playerStateMachine = PlayerStateMachine.STUN;
        yield return new WaitForSeconds(stunDuration);
        isElectrocut = false;
        m_playerStateMachine = PlayerStateMachine.IDLE;
    }

    public void ResetState()
    {
        m_attack.isAttacking = false;
        m_isRolling = false;
        m_attack.m_collider.enabled = false;

        isAttacking = false;

        if (!m_moving)
        {
            isMoving = false;
        }

        isRolling = false;
        normalCollider.center = normalCenter;
        normalCollider.height = normalHeight;
        collect = false;
        isThrowing = false;
        isElectrocut = false;
        isHurt = false;
        startTalking = false;
        isTalking = false;
    }

    #endregion

    #region INTERACTION

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

    public void EnterDialogue()
    {
        rb.velocity = Vector3.zero;
        m_controlState = ControlState.DIALOGUE;
    }

    public void ExitDialogue()
    {
        m_controlState = ControlState.NORMAL;
    }

    private void RollInput(InputAction.CallbackContext rollInput)
    {
        if (rollInput.started)
        {
            buttonAPressed = true;
        }

        if (rollInput.canceled)
        {
            buttonAPressed = false;
        }
    }

    #endregion

    #region VISUAL

    public void SetHat()
    {
        foreach (Hat h in hats)
        {
            if (h.hatObj == currentHat)
            {
                h.hatObj.SetActive(true);
            }
            else
            {
                h.hatObj.SetActive(false);
            }
        }
    }

    public void LoadVFX(ParticleSystem effect, Transform spawnT)
    {
        Destroy(Instantiate(effect, spawnT.position, Quaternion.identity).gameObject, 3); 
    }

    #endregion

    #region GAME

    public void RespawnPlayer()
    {
        ResetState();
        isDead = false;
        currentLifePoint = maxLifePoint = baseLifePoint;
        Rewind();
        rope.rewinding = true;

        if (SceneManager.GetActiveScene().name == "MAP_Boss_BackUp")
        {
            SceneManager.LoadScene("MAP_Boss_BackUp");
        }
        else
        {
            GameManager.instance.BackToCheckpoint();
        }

        GameManager.instance.ui.UpdateHealth();
        GameManager.instance.EnableAllEnemy();

        if (GameManager.instance.arenaParc != null)
        {
            GameManager.instance.arenaParc.ResetArena();
        }


        foreach (AIBrain a in GameManager.instance.enemyList)
        {
            a.isAggro = false;
        }
    }

    IEnumerator WaitForRespawn()
    {
        yield return new WaitForSeconds(3);
        RespawnPlayer();
    }

    public void OnRespawn()
    {
        transform.position = GameManager.instance.lastCheckpoint.respawnPoint.position;
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

    #endregion

    public void PlaySFX(string soundName)
    {
        foreach (AudioManager.Sounds s in AudioManager.instance.playerSounds)
        {
            if (s.soundName == soundName)
            {
                if (s.loop)
                {
                    SoundManager.PlayFx(s.clip, loop: true, volume: s.volume);
                }
                else
                {
                    SoundManager.PlayOnce(s.clip, volume: s.volume, mixer: SoundManager.FxAudioMixer);
                }
            }
        }
    }

    public void StopSFX(string soundName)
    {
        foreach (AudioManager.Sounds s in AudioManager.instance.playerSounds)
        {
            if (s.soundName == soundName)
            {
                SoundManager.StopFx(s.clip);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Item item = other.GetComponent<Item>();

        if (item && !item.distributed)
        {
            collectVFX.Play();
            GameManager.instance.RumbleConstant(.2f, .3f, .5f);

            switch (item.affectedValue)
            {
                case "Health":
                    if (currentLifePoint < maxLifePoint)
                    {
                        currentLifePoint += item.valuePercentage;
                    }

                    GameManager.instance.ui.UpdateHealth();
                    break;

                case "Rope":
                    rope.maximumLenght += item.valuePercentage;
                    break;

                case "Gloves":
                    gloves = true;
                    break;

                case "Hat":
                    foreach (Hat hats in hats)
                    {
                        if (item.itemName == hats.hatName)
                        {
                            hats.collected = true;
                            GameManager.instance.ui.UpdateHat();
                        }
                    }

                    break;
                case "Lore":
                    foreach (UIManager.LoreItem uiLore in GameManager.instance.ui.LoreItems)
                    {
                        if (item.itemName == uiLore.loreName)
                        {
                            uiLore.collected = true;
                            GameManager.instance.ui.UpdateLore();
                        }
                    }

                    break;
            }

            item.height = 1;
            item.distributed = true;
            if (item.PnjDialoguesManager) item.PnjDialoguesManager.StartDialogue();
            else Destroy(item.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Grappin>() && inputInteractPushed &&
            rope.maximumLenght >= other.GetComponent<Grappin>().ropeSizeNecessary)
        {
            other.GetComponent<Grappin>().StartGrappin();
            
            inputInteractPushed = false;
        }
        else if (other.GetComponent<Grappin>() && inputInteractPushed &&
                 rope.maximumLenght < other.GetComponent<Grappin>().ropeSizeNecessary)
        {
            other.GetComponent<PnjDialoguesManager>().StartDialogue();
        }
    }

    private void ClearAimList()
    {
        foreach (ValueTrack obj in GameManager.instance.grippableObj)
        {
            if (obj.meshRenderer == null || obj == null)
            {
                GameManager.instance.grippableObj.Remove(obj);
            }

            if (obj.meshRenderer == null) return;
            obj.meshRenderer.material.SetFloat("_EnableOutline", 0);
            if (!obj.isEnemy) continue;
            obj.gameObject.GetComponent<AIBrain>().modelAggroMat.SetFloat("_EnableOutline", 0);
            obj.gameObject.GetComponent<AIBrain>().modelNonAggroMat.SetFloat("_EnableOutline", 0);
        }
    }

    public void ChangeSpeedPlayer()
    {
        if (isSpeedBoost)
        {
            moveSpeed = 6.8f;
            speedBoostText.SetActive(false);
            isSpeedBoost = false;
        }
        else
        {
            moveSpeed = 20f;
            speedBoostText.SetActive(true);
            isSpeedBoost = true;
        }
    }

    public void SetGodMode()
    {
        if (isInvincible)
        {
            isInvincible = false;
            godModeText.SetActive(false);
        }
        else
        {
            isInvincible = true;
            godModeText.SetActive(true);
        }
    }


    #region SETUP

    private void OnEnable()
    {
        if (m_inputController != null) m_inputController.Enable();
    }

    private void OnDisable()
    {
        if (m_inputController != null) m_inputController.Disable();
    }

    #endregion



    void OnLeftTrigger()
    {
        transform.position = new Vector3(7.04f, 8.28f, -10.3f);
        SceneManager.LoadScene("MAP_Boss_BackUp");
    }
}

public enum ActionType
{
    StatusQuo,
    Aiming,
    Throwing,
    RopeAttached
}