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
    [FormerlySerializedAs("m_cameraPos")] public Transform cameraPos;
    [FormerlySerializedAs("m_cameraZoom")] public float cameraZoom;

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
        transform.position = Vector3.Lerp(transform.position,cameraPos.position, 0.05f);
        transform.rotation = Quaternion.Lerp(transform.rotation,cameraPos.rotation,0.05f);
        m_camera.orthographicSize = Mathf.Lerp(m_camera.orthographicSize, cameraZoom, 0.05f);

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
