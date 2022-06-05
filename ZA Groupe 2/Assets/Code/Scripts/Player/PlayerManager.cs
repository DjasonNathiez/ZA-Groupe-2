using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[SuppressMessage("ReSharper", "CheckNamespace")]
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance; //Singleton

    public GameObject VFXPoufpouf;

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
    public string state = "StatusQuo";
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
    
    //Inventory
    [Header("Inventory")]
    public bool gloves;
    public string startHat;
    public GameObject currentHat;
    public MeshFilter hatMesh;
    public MeshRenderer HatMeshRenderer;
    public Hat[] hats;
    
    [Serializable] public class Hat
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

    private bool isDead;


    //Animations
    private static readonly int AttackSpeed = Animator.StringToHash("AttackSpeed");
    private static readonly int Moving = Animator.StringToHash("Moving");
    private float attackCD;
    private float currentAttackCD;

    //Attacks
    [SerializeField] private GameObject visuthrow;

    #endregion

    #region VFX

    [Header("VFX")] 
    public ParticleSystem attackVFX;
    public ParticleSystem hurtVFX;
    public ParticleSystem rollVFX;

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

        attackCD = attackAnimClip.length;

        //Roll Animation Curve Setup
        rollAnimationCurve.keys[rollAnimationCurve.length - 1].time = rollAnimClip.length;
        m_acTimer = rollAnimationCurve.keys[rollAnimationCurve.length - 1].time;
        StartCoroutine(RadarEventTimer());
      
    

        GameManager.instance.ui.UpdateHat();
    }

    //GameStats
    IEnumerator RadarEventTimer()
    {
        GameStatsRecorder.Instance.RegisterEvent(new GameStatsLineTemplate(transform.position, "PlayerRadar"));
        
        yield return new WaitForSeconds(1f);

        StartCoroutine(RadarEventTimer());
    }

    private void Update()
    {
        Cursor.visible = m_playerInput.currentControlScheme == "Keyboard&Mouse"; //add a locker

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


            if (!m_attack.isAttacking)
            {
                if (state != "Aiming" && state != "Throw")
                    rb.velocity = Quaternion.Euler(0, -45, 0) * new Vector3(m_moveDirection.x * m_speed, rb.velocity.y,
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

            if (state == "Throw")
            {
             
                throwingWeapon.transform.Translate(direction * (Time.deltaTime * throwingSpeed));

            }

            if (state == "Aiming")
            {
                if (m_inputController.Player.Move.ReadValue<Vector2>() != Vector2.zero)
                {
                    List<ValueTrack> reachable = new List<ValueTrack>(0);
                    
                    foreach (ValueTrack obj in GameManager.instance.grippableObj)
                    {
                        if (obj == null || obj.GetComponent<ValueTrack>() == null) return;
                        if (Vector2.SqrMagnitude(new Vector2(obj.transform.position.x, obj.transform.position.z) - new Vector2(transform.position.x, transform.position.z)) < Mathf.Clamp(rope.maximumLenght ,0,12) * Mathf.Clamp(rope.maximumLenght ,0,12)  && obj.transform.position.y > transform.position.y - 1 && obj.transform.position.y < transform.position.y + 1)
                        {
                            reachable.Add(obj);
                        }
                    }
                
                    ValueTrack nearest = null;
                    float angle = aimHelpAngle;
                    Vector2 test = Vector2.Perpendicular(m_inputController.Player.Move.ReadValue<Vector2>()) + m_inputController.Player.Move.ReadValue<Vector2>();
                    foreach (ValueTrack obj in reachable)
                    {
                        if(Vector2.Angle(test,new Vector2(obj.transform.position.x,obj.transform.position.z) - new Vector2(transform.position.x,transform.position.z))< angle)
                        {
                            angle = Vector2.Angle(test, new Vector2(obj.transform.position.x, obj.transform.position.z) - new Vector2(transform.position.x, transform.position.z));
                            nearest = obj;
                        }
                    }

                    if (nearest != null)
                    {
                        transform.rotation = Quaternion.LookRotation(new Vector3(nearest.transform.position.x, 0, nearest.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z));
                        foreach (ValueTrack obj in reachable)
                        {
                            if (nearest == obj && obj.meshRenderer != null)
                            {
                                if (obj.isEnemy)
                                {
                                    obj.meshRenderer.material.SetFloat("_EnableOutline",1);
                                    obj.gameObject.GetComponent<AIBrain>().modelAggroMat.SetFloat("_EnableOutline", 1);
                                    obj.gameObject.GetComponent<AIBrain>().modelNonAggroMat.SetFloat("_EnableOutline", 1);
                                }
                                else obj.meshRenderer.material.SetFloat("_EnableOutline",1);
                            }
                            else if(nearest != obj && obj.meshRenderer != null)
                            {
                                obj.meshRenderer.material.SetFloat("_EnableOutline",0);
                                
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
                                obj.meshRenderer.material.SetFloat("_EnableOutline",0);
                                
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
                        if (obj.meshRenderer != null) obj.meshRenderer.material.SetFloat("_EnableOutline",0);
                        
                        if (!obj.isEnemy) continue;
                        if (obj == null) return;
                        obj.gameObject.GetComponent<AIBrain>().modelAggroMat.SetFloat("_EnableOutline", 0);
                        obj.gameObject.GetComponent<AIBrain>().modelNonAggroMat.SetFloat("_EnableOutline", 0);
                    }
                }
            }

            if (state != "Aiming")
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
            if (!m_isRolling && !m_attack.isAttacking && !isDead && !isAttacking && state == "StatusQuo")
            {
                m_canRoll = false;
                m_attack.isAttacking = true;
                m_attack.m_collider.enabled = true;
                m_attack.canHurt = true;
                
                GameStatsRecorder.Instance.RegisterEvent(new GameStatsLineTemplate(transform.position, "attack"));
                
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
        if (state == "Aiming")
        {
            isThrowing = true;
            
            throwingWeapon.SetActive(true);
            throwingWeapon.transform.position = transform.position + transform.forward * 0.5f;
            throwingWeapon.transform.LookAt(throwingWeapon.transform.position + transform.forward);
            
            direction = Vector3.forward;

            GameStatsRecorder.Instance.RegisterEvent(new GameStatsLineTemplate(transform.position, "cord"));
            state = "Throw";
            playerThrowingWeapon.SetActive(false);
            rope.enabled = true;
            rope.rope.gameObject.SetActive(true);
            PlaySFX("P_Throw");
            PlaySFX("P_RopeThrow");
        }
    }

    public void Rewind()
    {
        if (state == "Rope" || state == "Throw")
        {
            isThrowing = false;
            rope.rewinding = true;
            PlaySFX("P_Rewind");
        }
    }
    
    public void OnRange()
    {
        if (!isDead)
        {
            switch (state)
            {
                case "StatusQuo":
                    if (!m_attack.isAttacking && !m_isRolling)
                    {
                        PlayerManager.instance.rb.velocity = Vector3.zero;
                        ClearAimList();
                        state = "Aiming";
                        visuthrow.SetActive(true);
                    }

                    break;
                case "Throw":
                    Rewind();
                    break;
                case "Rope":
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
                case "Aiming":
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
        m_moveDirection = new Vector3(moveInput.ReadValue<Vector2>().x, 0, moveInput.ReadValue<Vector2>().y);
        move = new Vector3(moveInput.ReadValue<Vector2>().x, 0, moveInput.ReadValue<Vector2>().y);

        if (moveInput.performed)
        { 
       
            if (!m_isRolling && !m_attack.isAttacking && !isDead)
            {
                isMoving = true;
                
                if (poufpoufTimer < poufpoufTime)
                {
                    poufpoufTimer += Time.deltaTime;
                }
                else
                {
                    poufpoufTimer = 0;
                    poufpoufInstantiated = true;
                    GameObject go = Instantiate(VFXPoufpouf, transform.position + transform.TransformVector( poufpoufOffset), Quaternion.identity);
                    go.transform.parent = this.GameObject().transform;
                }
            }
         


            m_moving = true;

            //Rotation
            if (!isDead)
            {
                Quaternion lookRotation = Quaternion.LookRotation(Quaternion.Euler(0, -45, 0) * m_moveDirection);
                rb.MoveRotation(lookRotation);
            }


            // m_lookRot = Quaternion.LookRotation(m_moveDirection);
            // rb.DORotate(m_lookRot.eulerAngles, rotationSpeed);

            //FX

            //Instantiate(Poufpouf, transform.position, Quaternion.identity);
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
        
        if (roll.started)
        {
            if (!m_attack.isAttacking && !isDead && isMoving)
            {
                if (m_canRoll)
                {
                    GameStatsRecorder.Instance.RegisterEvent(new GameStatsLineTemplate(transform.position, "roll"));
                    rollVFX.Play();
                    PlaySFX("P_Roll");
                    isRolling = true;
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
                PlaySFX("P_Hurt");
                hurtVFX.Play();
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

    public void SetHat(GameObject hatSelected)
    {
        hatMesh.mesh = hatSelected.GetComponent<MeshFilter>().mesh;
        HatMeshRenderer.material = hatSelected.GetComponent<MeshRenderer>().material;
    }
    
    public void LoadVFX(ParticleSystem effect)
    {
        Instantiate(effect, transform.position, Quaternion.identity);
    }

    #endregion

    #region GAME
    
    public void RespawnPlayer()
    {
       
        GameManager.instance.BackToCheckpoint();
        ResetState();
        isDead = false;
        currentLifePoint = maxLifePoint = baseLifePoint;
        GameManager.instance.ui.UpdateHealth();
        GameManager.instance.EnableAllEnemy();
        
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
                    SoundManager.PlayFx(s.clip,loop: true, volume: s.volume);
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
            switch (item.affectedValue)
            {
                case "Health":
                    currentLifePoint += item.valuePercentage;
                  
                    break;

                case "Rope":
                    rope.maximumLenght += item.valuePercentage;
                    break;
                
                case "Gloves":
                    gloves = true;
                    Debug.Log("gloves is " + gloves);
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
            }
            item.height = 1;
            item.distributed = true;
            item.PnjDialoguesManager.StartDialogue();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Grappin>() && inputInteractPushed &&
            rope.maximumLenght >= other.GetComponent<Grappin>().ropeSizeNecessary)
        {
            transform.DOMove(other.GetComponent<Grappin>().pointToGo.position, grappleFlySpeed);
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
            if (obj.meshRenderer == null || obj == null) GameManager.instance.grippableObj.Remove(obj);
            if (obj.meshRenderer == null) return;
            obj.meshRenderer.material.SetFloat("_EnableOutline",0);
            if (!obj.isEnemy) continue;
            obj.gameObject.GetComponent<AIBrain>().modelAggroMat.SetFloat("_EnableOutline", 0);
            obj.gameObject.GetComponent<AIBrain>().modelNonAggroMat.SetFloat("_EnableOutline", 0);
        }
    }

    public void ChangeSpeedPlayer()
    {
        moveSpeed = 20;
    }
   
    
    

    #region SETUP

    private void OnEnable()
    {
        m_inputController.Enable();
    }

    private void OnDisable()
    {
        m_inputController.Disable();
    }

    #endregion
    

    
}