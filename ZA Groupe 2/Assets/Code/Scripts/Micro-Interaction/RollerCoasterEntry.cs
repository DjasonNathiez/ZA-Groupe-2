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
    [SerializeField] private QuadraticCurve curve;
    [SerializeField] private Transform SpawnPointEnd;
    public Vector3 pos;

    public GameObject statue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rollerCoaster.points = curve.points;
            rollerCoaster.transform.position = curve.followCurve.points[0].point;
            rollerCoaster.moving = true;
            rollerCoaster.cam = cam;
            cam.playerFocused = false;
            PlayerManager.instance.transform.position = pos;
            PlayerManager.instance.EnterDialogue();

            if (AudioManager.instance.secretSoundActivated)
            {
                AudioManager.instance.SetMusic("Secret_Sound");
                statue.GetComponent<Animator>().Play("Dance");
            }
        }
    }

    private void Start()
    {
        cam = PlayerManager.instance.cameraController;
    }
}
