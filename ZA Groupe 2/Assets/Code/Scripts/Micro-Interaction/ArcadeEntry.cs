using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ArcadeEntry : MonoBehaviour
{
    [SerializeField] private MiniGameManager miniGameManager;
    [SerializeField] private CameraController cam;
    [SerializeField] private GameObject camArcade;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private GameObject button;
    [SerializeField] private bool check;
    [SerializeField] private float zoom;
    [SerializeField] private float nearPlane;
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
            camArcade.transform.position = new Vector3(miniGameManager.transform.position.x,miniGameManager.transform.position.y, -1);
            PlayerManager.instance.EnterDialogue();
            miniGameManager.onArcade = true;
            miniGameManager.game = game;
            miniGameManager.cam = cam;
            button.SetActive(false);
            miniGameManager.screenMaterial.material.color = Color.white;
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
