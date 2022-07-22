using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public enum WeightClass
{
    NULL,
    LIGHT,
    MEDIUM,
    HEAVY,
}

public class ValueTrack : MonoBehaviour
{
    [Header("Usefull Attributs")] public bool isEnemy;
    public bool canActivatePressurePlate;
    public bool canBePropulsed;

    [Header("Outline Attributs")] public WeightClass weightClass;
    public MeshRenderer meshRenderer;
    public Vector3 posCam;
    public Vector3 rotCam;
    public float zoomCam;
    public bool moveCam;
    public RigidbodyConstraints constraints;

    [Header("Visual")] public GameObject splashVFX;
    public VisualEffect trailVFX;

    private void Start()
    {
        if (!meshRenderer) meshRenderer = GetComponent<MeshRenderer>();
        constraints = GetComponent<Rigidbody>().constraints;
        if (GetComponentInChildren<VisualEffect>())
        {
            trailVFX = GetComponentInChildren<VisualEffect>();
            trailVFX.Stop();
            trailVFX.gameObject.SetActive(false);
        }

        if (GetComponent<VisualEffect>()) Debug.LogWarning(name + " !!!!");
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.layer)
        {
            // Water Layer
            case 4:

                ContactPoint contact = collision.GetContact(0);

                //Ignore collision on water
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());

                //Add the floating movement
                meshRenderer.material.SetFloat("_UseMovingVertices", 1);
                meshRenderer.material.SetVector("_SpeedMoveAxis", new Vector3(0, 0, 1));
                meshRenderer.material.SetVector("_FrequencyAxis", new Vector3(0, 0, 1.3f));
                meshRenderer.material.SetVector("_AmplitudeAxis", new Vector3(0, 0, 0.2f));
                meshRenderer.material.SetVector("_AdjustPosAxis", new Vector3(0, 0, 0.1f));

                //Add Visual effect
                if (splashVFX)
                {
                    Instantiate(splashVFX, contact.point, Quaternion.identity);
                }

                break;

            //Invisible Wall

            case 9:
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(),
                    gameObject.GetComponent<Collider>());
                break;
        }
    }
}