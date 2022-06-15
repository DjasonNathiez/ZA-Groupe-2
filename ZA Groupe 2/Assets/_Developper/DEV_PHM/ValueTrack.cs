using System;using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeightClass
{
    NULL,
    LIGHT,
    MEDIUM,
    HEAVY,
}
public class ValueTrack : MonoBehaviour
{
    [Header("Usefull Attributs")]
    public bool isEnemy;
    public bool canActivatePressurePlate;
    
    [Header("Outline Attributs")]
    public WeightClass weightClass;
    public MeshRenderer meshRenderer;
    public Vector3 posCam;
    public Vector3 rotCam;
    public bool moveCam;
    public ParticleSystem splashVFX;
    public bool onWater;

    public float speed;
    public float amplitude;
    public float frequency;
    public float adjust;

    public float angle;

    private void Start()
    {
        if(!meshRenderer) meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Update()
    {
        if (onWater)
        {
            angle = transform.rotation.x - GameManager.instance.transform.rotation.x;
            
            if (angle > 0)
            {
                meshRenderer.material.SetVector("_SpeedMoveAxis", new Vector3(0,speed,0));
                meshRenderer.material.SetVector("_FrequencyAxis", new Vector3(0,frequency,0));
                meshRenderer.material.SetVector("_AmplitudeAxis", new Vector3(0,amplitude,0));
                meshRenderer.material.SetVector("_AdjustPosAxis", new Vector3(0,adjust,0));
            }
            else
            {
           
                meshRenderer.material.SetVector("_SpeedMoveAxis", new Vector3(0,0,speed));
                meshRenderer.material.SetVector("_FrequencyAxis", new Vector3(0,0,frequency));
                meshRenderer.material.SetVector("_AmplitudeAxis", new Vector3(0,0,amplitude));
                meshRenderer.material.SetVector("_AdjustPosAxis", new Vector3(0,0,adjust));
            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 4)
        {
            ContactPoint contact = collision.GetContact(0);
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
            
            onWater = true;
            meshRenderer.material.SetFloat("_UseMovingVertices", 1);
            meshRenderer.material.SetVector("_SpeedMoveAxis", new Vector3(0,0,1));
            meshRenderer.material.SetVector("_FrequencyAxis", new Vector3(0,0,1.3f));
            meshRenderer.material.SetVector("_AmplitudeAxis", new Vector3(0,0,0.2f));
            meshRenderer.material.SetVector("_AdjustPosAxis", new Vector3(0,0,0.1f));
            Instantiate(splashVFX, contact.point, Quaternion.identity);
        }
        
        if (collision.gameObject.layer == 9)
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }
}


