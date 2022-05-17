using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    
    private GameObject m_player;
    private Camera m_camera;
    public bool playerFocused;
    public Transform cameraPos;
    public float cameraZoom;
    public float panSpeed = 0.5f;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        
        InitializeCamera();
    }

    public void InitializeCamera()
    {
        m_player = GameObject.FindGameObjectWithTag("Player"); 
        m_camera = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position,cameraPos.position, Time.deltaTime*5*panSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation,cameraPos.rotation,Time.deltaTime*5*panSpeed);
        m_camera.orthographicSize = Mathf.Lerp(m_camera.orthographicSize, cameraZoom, Time.deltaTime*5*panSpeed);

        if (playerFocused && m_player)
        {
            cameraPos.position = m_player.transform.position;   
        }
    }

    /*private void OnValidate()
    {
        SetCameraBasePosDebug();
    }

    public void SetCameraBasePosDebug()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_camera = GetComponent<Camera>();
        
        transform.position = m_cameraPos.position;
        transform.rotation = m_cameraPos.rotation;
        m_camera.orthographicSize = m_cameraZoom;

        if (playerFocused)
        {
            m_cameraPos.position = m_player.transform.position;   
        }
    }*/
}
