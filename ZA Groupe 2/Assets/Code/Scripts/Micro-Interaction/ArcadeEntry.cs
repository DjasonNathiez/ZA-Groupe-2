using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ArcadeEntry : MonoBehaviour
{
    [FormerlySerializedAs("m_arcadeController")] [SerializeField] private ArcadeController arcadeController;
    [FormerlySerializedAs("m_cam")] [SerializeField] private CameraController cam;
    [SerializeField] private Vector3 pos;
    [SerializeField] private Vector3 rotation;
    [FormerlySerializedAs("m_Button")] [SerializeField] private GameObject button;
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
            button.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && PlayerManager.instance.inputInteractPushed && !check)
        {
            check = true;
            arcadeController.onArcade = true;
            cam.playerFocused = false;
            cam.cameraPos.position  = transform.position + pos;
            cam.cameraPos.rotation = Quaternion.Euler(rotation);
            cam.cameraZoom = zoom;
            PlayerManager.instance.moveSpeed = 0;
            arcadeController.bg.sprite = background;
            arcadeController.title.GetComponent<SpriteRenderer>().sprite = title;
            arcadeController.screen = screen;
            arcadeController.game = game;
        }

        if (other.CompareTag("Player") && !PlayerManager.instance.inputInteractPushed && check)
        {
            check = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            button.SetActive(false);
        }
    }
}
