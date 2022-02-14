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

    #region Input Settings
    [Header("Input Settings")]

    //only axis
    public MoveInput moveInput;
    public enum MoveInput{None, LeftStick, RightStick}
    
    
    //only buttons
    public AttackInput attackInput;
    public enum AttackInput{None, UpButton, DownButton, LeftButton, RightButton, DpadUp, DpadDown, DpadLeft, DpadRight, LeftStickButton, RightStickButton, LeftShoulder, RightShoulder, StartButton, SelectButton}
    
    
    //only triggers
    
    
    #endregion
    
    [Header("Current Statistics")] 
    public float speed;
    public int attackDamage;
    public float attackSpeed;
    
    private Vector3 m_moveDirection;
    
    #region Inputs Callback Value

    [Header("Buttons Value")] 
    
    //Left Button
    [HideInInspector] public float leftButtonValue;
    [HideInInspector]public bool leftButtonStarted;
    [HideInInspector]public bool leftButtonPerformed;
    [HideInInspector]public bool leftButtonCancel;

    //Right Button
    [HideInInspector]public float rightButtonValue;
    [HideInInspector]public bool rightButtonStarted;
    [HideInInspector]public bool rightButtonPerformed;
    [HideInInspector]public bool rightButtonCancel;
    
    //Up Button
    [HideInInspector]public float upButtonValue;
    [HideInInspector]public bool upButtonStarted;
    [HideInInspector]public bool upButtonPerformed;
    [HideInInspector]public bool upButtonCancel;
    
    //Down Button
    [HideInInspector]public float downButtonValue;
    [HideInInspector]public bool downButtonStarted;
    [HideInInspector]public bool downButtonPerformed;
    [HideInInspector]public bool downButtonCancel;
    
    //Dpad Left Button
    [HideInInspector]public float dpadLeftButtonValue;
    [HideInInspector]public bool dpadLeftButtonStarted;
    [HideInInspector]public bool dpadLeftButtonPerformed;
    [HideInInspector]public bool dpadLeftButtonCancel;
    
    //Dpad Right Button
    [HideInInspector]public float dpadRightButtonValue;
    [HideInInspector]public bool dpadRightButtonStarted;
    [HideInInspector]public bool dpadRightButtonPerformed;
    [HideInInspector]public bool dpadRightButtonCancel;
    
    //DPad Up Button
    [HideInInspector]public float dpadUpButtonValue;
    [HideInInspector]public bool dpadUpButtonStarted;
    [HideInInspector]public bool dpadUpButtonPerformed;
    [HideInInspector]public bool dpadUpButtonCancel;
    
    //Dpad Down Button
    [HideInInspector]public float dpadDownButtonValue;
    [HideInInspector]public bool dpadDownButtonStarted;
    [HideInInspector]public bool dpadDownButtonPerformed;
    [HideInInspector]public bool dpadDownButtonCancel;
    
    //Start Button
    [HideInInspector]public float startButtonValue;
    [HideInInspector]public bool startButtonStarted;
    [HideInInspector]public bool startButtonPerformed;
    [HideInInspector]public bool startButtonCancel;
    
    //Select Button
    [HideInInspector]public float selectButtonValue;
    [HideInInspector]public bool selectButtonStarted;
    [HideInInspector]public bool selectButtonPerformed;
    [HideInInspector]public bool selectButtonCancel;
    
    //Left Stick Button
    [HideInInspector]public float leftStickButtonValue;
    [HideInInspector]public bool leftStickButtonStarted;
    [HideInInspector]public bool leftStickButtonPerformed;
    [HideInInspector]public bool leftStickButtonCancel;
    
    //Right Stick Button
    [HideInInspector]public float rightStickButtonValue;
    [HideInInspector]public bool rightStickButtonStarted;
    [HideInInspector]public bool rightStickButtonPerformed;
    [HideInInspector]public bool rightStickButtonCancel;
    
    //Left Shoulder Button
    [HideInInspector]public float leftShoulderButtonValue;
    [HideInInspector]public bool leftShoulderButtonStarted;
    [HideInInspector]public bool leftShoulderButtonPerformed;
    [HideInInspector]public bool leftShoulderButtonCancel;
    
    //Right Shoulder Button
    [HideInInspector]public float rightShoulderButtonValue;
    [HideInInspector]public bool rightShoulderButtonStarted;
    [HideInInspector]public bool rightShoulderButtonPerformed;
    [HideInInspector]public bool rightShoulderButtonCancel;
    
    [Header("Axis Value")] 
    
    //Left Stick
    [HideInInspector]public Vector2 leftStickAxis;
    [HideInInspector]public bool leftAxisStarted;
    [HideInInspector]public bool leftAxisPerformed;
    [HideInInspector]public bool leftAxisCancel;
    
    //Right Stick
    [HideInInspector]public Vector2 rightStickAxis;
    [HideInInspector] public bool rightAxisStarted;
    [HideInInspector] public bool rightAxisPerformed;
    [HideInInspector] public bool rightAxisCancel;

    [Header("Triggers Value")] 
    
    //Right Trigger
    [HideInInspector]public float rightTriggerValue;
    [HideInInspector] public bool rightTriggerStarted;
    [HideInInspector] public bool rightTriggerPerformed;
    [HideInInspector] public bool rightTriggerCancel;
    
    //Left Trigger
    [HideInInspector]public float leftTriggerValue;
    [HideInInspector] public bool leftTriggerStarted;
    [HideInInspector] public bool leftTriggerPerformed;
    [HideInInspector] public bool leftTriggerCancel;
    
    #endregion
    
    private void Awake()
    {
        m_inputController = new InputController();
        m_animator = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        #region INITIALIZE INPUT

        //BUTTONS
        m_inputController.Player.UpButton.started += UpButton;
        m_inputController.Player.UpButton.performed += UpButton;
        m_inputController.Player.UpButton.canceled += UpButton;
        
        m_inputController.Player.LeftButton.started += LeftButton;
        m_inputController.Player.LeftButton.performed += LeftButton;
        m_inputController.Player.LeftButton.canceled += LeftButton;
        
        m_inputController.Player.RightButton.started += RightButton;
        m_inputController.Player.RightButton.performed += RightButton;
        m_inputController.Player.RightButton.canceled += RightButton;
        
        m_inputController.Player.DownButton.started += DownButton;
        m_inputController.Player.DownButton.performed += DownButton;
        m_inputController.Player.DownButton.canceled += DownButton;

        m_inputController.Player.DPadUp.started += DPadUp;
        m_inputController.Player.DPadUp.performed += DPadUp;
        m_inputController.Player.DPadUp.canceled += DPadUp;
        
        m_inputController.Player.DPadDown.started += DPadDown;
        m_inputController.Player.DPadDown.performed += DPadDown;
        m_inputController.Player.DPadDown.canceled += DPadDown;
        
        m_inputController.Player.DPadLeft.started += DPadLeft;
        m_inputController.Player.DPadLeft.performed += DPadLeft;
        m_inputController.Player.DPadLeft.canceled += DPadLeft;
        
        m_inputController.Player.DPadRight.started += DPadRight;
        m_inputController.Player.DPadRight.performed += DPadRight;
        m_inputController.Player.DPadRight.canceled += DPadRight;

        m_inputController.Player.LeftStickButton.started += LeftStickButton;
        m_inputController.Player.LeftStickButton.performed += LeftStickButton;
        m_inputController.Player.LeftStickButton.canceled += LeftStickButton;
        
        m_inputController.Player.RightStickButton.started += RightStickButton;
        m_inputController.Player.RightStickButton.performed += RightStickButton;
        m_inputController.Player.RightStickButton.canceled += RightStickButton;
        
        m_inputController.Player.StartButton.started += StartButton;
        m_inputController.Player.StartButton.performed += StartButton;
        m_inputController.Player.StartButton.canceled += StartButton;

        m_inputController.Player.SelectButton.started += SelectButton;
        m_inputController.Player.SelectButton.performed += SelectButton;
        m_inputController.Player.SelectButton.canceled += SelectButton;
        
        m_inputController.Player.LeftShoulder.started += LeftShoulder;
        m_inputController.Player.LeftShoulder.performed += LeftShoulder;
        m_inputController.Player.LeftShoulder.canceled += LeftShoulder;
        
        m_inputController.Player.RightShoulder.started += RightShoulder;
        m_inputController.Player.RightShoulder.performed += RightShoulder;
        m_inputController.Player.RightShoulder.canceled += RightShoulder;
        
        //AXIS
        m_inputController.Player.LeftStick.started += LeftStickAxis;
        m_inputController.Player.LeftStick.performed += LeftStickAxis;
        m_inputController.Player.LeftStick.canceled += LeftStickAxis;
        
        m_inputController.Player.RightStick.started += RightStick;
        m_inputController.Player.RightStick.performed += RightStick;
        m_inputController.Player.RightStick.canceled += RightStick;
        
        //TRIGGER
        
        m_inputController.Player.LeftTrigger.started += LeftTrigger;
        m_inputController.Player.LeftTrigger.performed += LeftTrigger;
        m_inputController.Player.LeftTrigger.canceled += LeftTrigger;
        
        m_inputController.Player.RightTrigger.started += RightTrigger;
        m_inputController.Player.RightTrigger.performed += RightTrigger;
        m_inputController.Player.RightTrigger.canceled += RightTrigger;

        #endregion
        
        AttributionInput();
        NormalMove();
    }

    #region READ VALUE INPUT FUNCTIONS
    private void RightTrigger(InputAction.CallbackContext rightTrigger)
    {
        rightTriggerValue = rightTrigger.ReadValue<float>();
        rightTriggerStarted = rightTrigger.started;
        rightTriggerPerformed = rightTrigger.performed;
        rightTriggerCancel = rightTrigger.canceled;
    }

    private void LeftTrigger(InputAction.CallbackContext leftTrigger)
    {
        leftTriggerValue = leftTrigger.ReadValue<float>();
        leftTriggerStarted = leftTrigger.started;
        leftTriggerPerformed = leftTrigger.performed;
        leftTriggerCancel = leftTrigger.canceled;
    }

    private void RightShoulder(InputAction.CallbackContext rightShoulder)
    {
        rightShoulderButtonValue = rightShoulder.ReadValue<float>();
        rightShoulderButtonStarted = rightShoulder.started;
        rightShoulderButtonPerformed = rightShoulder.performed;
        rightShoulderButtonCancel = rightShoulder.canceled;
    }

    private void LeftShoulder(InputAction.CallbackContext leftShoulder)
    {
        leftShoulderButtonValue = leftShoulder.ReadValue<float>();
        leftShoulderButtonStarted = leftShoulder.started;
        leftShoulderButtonPerformed = leftShoulder.performed;
        leftShoulderButtonCancel = leftShoulder.canceled;
    }

    private void SelectButton(InputAction.CallbackContext selectButton)
    {
        selectButtonValue = selectButton.ReadValue<float>();
        selectButtonStarted = selectButton.started;
        selectButtonPerformed = selectButton.performed;
        selectButtonCancel = selectButton.canceled;
    }

    private void StartButton(InputAction.CallbackContext startButton)
    {
        startButtonValue = startButton.ReadValue<float>();
        startButtonStarted = startButton.started;
        startButtonPerformed = startButton.performed;
        startButtonCancel = startButton.canceled;
    }

    private void RightStickButton(InputAction.CallbackContext rightStickButton)
    {
        rightStickButtonValue = rightStickButton.ReadValue<float>();
        rightStickButtonStarted = rightStickButton.started;
        rightStickButtonPerformed = rightStickButton.performed;
        rightStickButtonCancel = rightStickButton.canceled;
    }

    private void LeftStickButton(InputAction.CallbackContext leftStickButton)
    {
        leftStickButtonValue = leftStickButton.ReadValue<float>();
        leftStickButtonStarted = leftStickButton.started;
        leftStickButtonPerformed = leftStickButton.performed;
        leftStickButtonCancel = leftStickButton.canceled;
    }

    private void DPadRight(InputAction.CallbackContext dpadRight)
    {
        dpadRightButtonValue = dpadRight.ReadValue<float>();
        dpadRightButtonStarted = dpadRight.started;
        dpadRightButtonPerformed = dpadRight.performed;
        dpadRightButtonCancel = dpadRight.canceled;
    }

    private void DPadLeft(InputAction.CallbackContext dpadLeft)
    {
        dpadLeftButtonValue = dpadLeft.ReadValue<float>();
        dpadLeftButtonStarted = dpadLeft.started;
        dpadLeftButtonPerformed = dpadLeft.performed;
        dpadLeftButtonCancel = dpadLeft.canceled;
    }

    private void DPadDown(InputAction.CallbackContext dpadDown)
    {
        dpadDownButtonValue = dpadDown.ReadValue<float>();
        dpadDownButtonStarted = dpadDown.started;
        dpadDownButtonPerformed = dpadDown.performed;
        dpadDownButtonCancel = dpadDown.canceled;
    }

    private void DPadUp(InputAction.CallbackContext dpadUp)
    {
        dpadUpButtonValue = dpadUp.ReadValue<float>();
        dpadUpButtonStarted = dpadUp.started;
        dpadUpButtonPerformed = dpadUp.performed;
        dpadUpButtonCancel = dpadUp.canceled;
    }

    private void DownButton(InputAction.CallbackContext downButton)
    {
        downButtonValue = downButton.ReadValue<float>();
        downButtonStarted = downButton.started;
        downButtonPerformed = downButton.performed;
        downButtonCancel = downButton.canceled;
    }

    private void RightButton(InputAction.CallbackContext rightButton)
    {
        rightButtonValue = rightButton.ReadValue<float>();
        rightButtonStarted = rightButton.started;
        rightButtonPerformed = rightButton.performed;
        rightButtonCancel = rightButton.canceled;
    }

    private void RightStick(InputAction.CallbackContext rightStick)
    {
        rightStickAxis = rightStick.ReadValue<Vector2>();
        rightAxisStarted = rightStick.started;
        rightAxisPerformed = rightStick.performed;
        rightAxisCancel = rightStick.canceled;

    }

    private void LeftStickAxis(InputAction.CallbackContext leftStick)
    {
        leftStickAxis = leftStick.ReadValue<Vector2>();
        leftAxisStarted = leftStick.started;
        leftAxisPerformed = leftStick.performed;
        leftAxisCancel = leftStick.canceled;
    }

    private void LeftButton(InputAction.CallbackContext leftButton)
    {
        leftButtonValue = leftButton.ReadValue<float>();
        leftButtonStarted = leftButton.started;
        leftButtonPerformed = leftButton.performed;
        leftButtonCancel = leftButton.canceled;
    }

    private void UpButton(InputAction.CallbackContext upButton)
    {
        upButtonValue = upButton.ReadValue<float>();
        upButtonStarted = upButton.started;
        upButtonPerformed = upButton.performed;
        upButtonCancel = upButton.canceled;
    }
    
    #endregion

    private void AttributionInput()
    {
        switch (controlerState)
        {
            case ControlerState.NORMAL:

                #region Movement
                
                switch (moveInput)
                {
                    case MoveInput.None:
                        Debug.LogWarning("No input set for movements, please set it up");
                        break;
            
                    case MoveInput.LeftStick:
                        m_inputController.Player.LeftStick.performed += context => m_moveDirection = new Vector3(leftStickAxis.x, 0, leftStickAxis.y);

                        m_inputController.Player.LeftStick.canceled += context => m_moveDirection = Vector3.zero;
                        m_inputController.Player.LeftStick.canceled += context => m_rb.velocity = Vector3.zero;
                        break;
            
                    case MoveInput.RightStick:
                        m_inputController.Player.RightStick.performed += context => m_moveDirection = new Vector3(rightStickAxis.x, 0, rightStickAxis.y);
                        
                        m_inputController.Player.RightStick.canceled += context => m_moveDirection = Vector3.zero;
                        m_inputController.Player.RightStick.canceled += context => m_rb.velocity = Vector3.zero;
                        break;
                }
                #endregion

                #region Attack

                switch (attackInput)
                {
                    case AttackInput.None:
                        break;
            
                    case AttackInput.DownButton:
                        m_inputController.Player.DownButton.started += context => LoadAttack();
                        break;
            
                    case AttackInput.LeftButton:
                        m_inputController.Player.LeftButton.started += context => LoadAttack();
                        break;
                }

                #endregion
                
                break;
        }
      
    }
    
    private void NormalMove()
    {
        //move to the direction of the input by movement speed
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
