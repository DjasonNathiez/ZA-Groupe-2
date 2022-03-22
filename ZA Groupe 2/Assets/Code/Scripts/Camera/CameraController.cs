using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    
    private GameObject m_player;
    private Camera m_camera;
    public bool playerFocused;
    public Transform m_cameraPos;
    public float m_cameraZoom;

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
        
        m_player = GameObject.FindGameObjectWithTag("Player"); 
        m_camera = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position,m_cameraPos.position, 0.05f);
        transform.rotation = Quaternion.Lerp(transform.rotation,m_cameraPos.rotation,0.05f);
        m_camera.orthographicSize = Mathf.Lerp(m_camera.orthographicSize, m_cameraZoom, 0.05f);

        if (playerFocused)
        {
            m_cameraPos.position = m_player.transform.position;   
        }
    }

    private void OnValidate()
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
    }
}
