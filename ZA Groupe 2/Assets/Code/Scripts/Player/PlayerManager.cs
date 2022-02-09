using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private InputController m_inputController;
    private Rigidbody m_rb;
    
    public PlayerStateMachine playerStateMachine;
    public enum PlayerStateMachine { IDLE, MOVE, ATTACK };
    
    [Header("Current Statistics")] 
    public float speed;
    private Vector3 m_moveDirection;
    
    #region Inputs Callback Value

    [Header("Buttons Value")] 
    
    //Left Button
    [HideInInspector] public float leftButtonValue;
    [HideInInspector]public bool leftButtonStarted;
    [HideInInspector]public bool leftButtonPerformed;
    [HideInInspector]public bool leftButtonCancel;

    public LeftButtonAssign leftButtonAssign;
    public enum LeftButtonAssign{None, Attack}
    
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
    [HideInInspector]public Vector2 leftStickAxis;

    public LeftStickAxisAssign leftStickAxisAssign;
    public enum LeftStickAxisAssign{None, Move}
    
    [HideInInspector]public Vector2 rightStickAxis;
    
    public RightStickAxisAssign rightStickAxisAssign;
    public enum RightStickAxisAssign{None, Move}

    [Header("Triggers Value")] 
    [HideInInspector]public float rightTriggerValue;
    [HideInInspector]public float leftTriggerValue;
    
    #endregion
    
    private void Awake()
    {
        m_inputController = new InputController();
       
        m_rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        #region READ INPUT

        m_inputController.Player.UpButton.started += UpButton;
        m_inputController.Player.UpButton.performed += UpButton;
        m_inputController.Player.UpButton.canceled += UpButton;
        
        m_inputController.Player.LeftButton.started += LeftButton;
        m_inputController.Player.LeftButton.performed += LeftButton;
        m_inputController.Player.LeftButton.canceled += LeftButton;
        
        m_inputController.Player.LeftStick.started += LeftStickAxis;
        m_inputController.Player.LeftStick.performed += LeftStickAxis;
        m_inputController.Player.LeftStick.canceled += LeftStickAxis;
        
        m_inputController.Player.RightStick.started += RightStick;
        m_inputController.Player.RightStick.performed += RightStick;
        m_inputController.Player.RightStick.canceled += RightStick;
        
        #endregion
        
        Move();
    }

    private void RightStick(InputAction.CallbackContext rightStick)
    {
        rightStickAxis = rightStick.ReadValue<Vector2>();

        switch (rightStickAxisAssign)
        {
            case RightStickAxisAssign.None:
                break;
            
            case RightStickAxisAssign.Move:
                m_moveDirection = new Vector3(rightStickAxis.x, 0, rightStickAxis.y);

                if (rightStick.canceled)
                {
                    m_moveDirection = Vector3.zero;
                }
                break;
        }
    }

    private void LeftStickAxis(InputAction.CallbackContext leftStick)
    {
        leftStickAxis = leftStick.ReadValue<Vector2>();

        switch (leftStickAxisAssign)
        {
            case LeftStickAxisAssign.None:
                break;
            
            case LeftStickAxisAssign.Move:
                m_moveDirection = new Vector3(leftStickAxis.x, 0, leftStickAxis.y);

                if (leftStick.canceled)
                {
                    m_moveDirection = Vector3.zero;
                }
                break;
        }
    }

    private void LeftButton(InputAction.CallbackContext leftButton)
    {
        leftButtonValue = leftButton.ReadValue<float>();
        leftButtonStarted = leftButton.started;
        leftButtonPerformed = leftButton.performed;
        leftButtonCancel = leftButton.canceled;

        switch (leftButtonAssign)
        {
            case LeftButtonAssign.Attack:

                if (leftButton.started)
                {
                    Debug.Log("Attack performed on left button");
                }
                
                break;
        }
    }

    private void UpButton(InputAction.CallbackContext upButton)
    {
        upButtonValue = upButton.ReadValue<float>();
        upButtonStarted = upButton.started;
        upButtonPerformed = upButton.performed;
        upButtonCancel = upButton.canceled;
    }

    private void Move()
    {
        //move to the direction of the input by movement speed
        m_rb.velocity = m_moveDirection * speed;

        if (m_moveDirection != Vector3.zero)
        {
            //change the look direction to the last move direction
            Quaternion lookRotation = Quaternion.LookRotation(m_moveDirection); 
            m_rb.MoveRotation(lookRotation);
            
            //switch the state of player to MOVE, which means the player is moving
            playerStateMachine = PlayerStateMachine.MOVE;
        }
        else
        {
            //in case that the moveDirection Vector is equal to zero, this mean the player isn't moving so he back to the idle state
            playerStateMachine = PlayerStateMachine.IDLE;
        }
    }

    private void OnValidate()
    {
        if (leftStickAxisAssign == LeftStickAxisAssign.Move && rightStickAxisAssign == RightStickAxisAssign.Move)
        {
            Debug.LogWarning("Move is assign to the two sticks, move will be set to left stick by default");
            rightStickAxisAssign = RightStickAxisAssign.None;
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
