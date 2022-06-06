using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RollerCoasterEntry : MonoBehaviour
{
    public bool isDialoguing;
    [FormerlySerializedAs("m_rollerCoaster")] [SerializeField] private FollowCurve rollerCoaster;
    [FormerlySerializedAs("m_cam")] [SerializeField] private CameraController cam;
    [FormerlySerializedAs("m_Button")] [SerializeField] private GameObject button;
    [SerializeField] private bool check;
    [SerializeField] private Transform SpawnPointEnd;
    public Vector3 pos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rollerCoaster.moving = true;
            rollerCoaster.cam = cam;
            cam.playerFocused = false;
            PlayerManager.instance.transform.position = pos;
            PlayerManager.instance.EnterDialogue();
        }
    }

    private void Start()
    {
        cam = PlayerManager.instance.cameraController;
    }
}
