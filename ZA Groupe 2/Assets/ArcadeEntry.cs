using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeEntry : MonoBehaviour
{
    [SerializeField] private arcadeController m_arcadeController;
    [SerializeField] private CameraController m_cam;
    [SerializeField] private Vector3 pos;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private GameObject m_Button;
    [SerializeField] private bool check;
    [SerializeField] private float zoom;
    [SerializeField] private float nearPlane;
    [SerializeField] private Sprite background;
    [SerializeField] private Sprite title;
    [SerializeField] private GameObject screen;
    [SerializeField] private int game;

    private void Start()
    {
        screen.GetComponent<MeshRenderer>().material = new Material(screen.GetComponent<MeshRenderer>().material);
    }

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
            m_arcadeController.onArcade = true;
            m_cam.playerFocused = false;
            m_cam.m_cameraPos.position  = transform.position + pos;
            m_cam.m_cameraPos.rotation = Quaternion.Euler(rotation);
            m_cam.m_cameraZoom = zoom;
            PlayerManager.instance.moveSpeed = 0;
            m_arcadeController.bg.sprite = background;
            m_arcadeController.title.GetComponent<SpriteRenderer>().sprite = title;
            m_arcadeController.screen = screen;
            m_arcadeController.game = game;
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
