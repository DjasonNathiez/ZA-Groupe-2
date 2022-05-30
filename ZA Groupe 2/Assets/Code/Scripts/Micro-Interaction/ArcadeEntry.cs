using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ArcadeEntry : MonoBehaviour
{
    [SerializeField] private arcadeController arcadeController;
    [SerializeField] private CameraController cam;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private GameObject button;
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
        cam = PlayerManager.instance.cameraController;
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
            cam.playerFocused = false;
            cam.cameraPos.position  = transform.position;
            cam.cameraPos.rotation = Quaternion.Euler(rotation);
            cam.cameraZoom = zoom;
            
            PlayerManager.instance.EnterDialogue();
            arcadeController.onArcade = true;
            arcadeController.game = game;
            arcadeController.screen = screen;
            button.SetActive(false);
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
