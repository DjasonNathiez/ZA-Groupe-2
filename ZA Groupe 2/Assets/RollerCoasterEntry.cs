using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollerCoasterEntry : MonoBehaviour
{
    public bool isDialoguing;
    [SerializeField] private FollowCurve m_rollerCoaster;
    [SerializeField] private CameraController m_cam;
    [SerializeField] private GameObject m_Button;
    [SerializeField] private bool check;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_Button.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && PlayerManager.instance.inputInterractPushed && !check)
        {
            check = true;
            m_rollerCoaster.moving = true;
            m_rollerCoaster.cam = m_cam;
            m_cam.playerFocused = false;

        }

        if (other.CompareTag("Player") && !PlayerManager.instance.inputInterractPushed && check)
        {
            check = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_Button.SetActive(false);
        }
    }
    
}
