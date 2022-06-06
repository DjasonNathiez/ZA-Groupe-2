using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public int numberofCurrent;
    public bool isActivate;
    public Door[] doors;
    public bool multiActivate;
    public Material onMat;
    public Material offMat;
    public MeshRenderer meshRenderer;
    
    public AudioClip enterPlateSound;
    public AudioClip exitPlateSound;
    public float volume;
    
    private void OnTriggerEnter(Collider other)
    {

        if (!other.gameObject.GetComponent<Rigidbody>()) return;
        if (!multiActivate)
        {
            if (other.CompareTag("Player"))
            {
                numberofCurrent++;
                if (!isActivate)
                {
                    foreach (Door door in doors) { door.keysValid++; }
                    AudioManager.instance.PlayEnvironment("PressurePlate_On");

                    isActivate = true;
                    meshRenderer.material = onMat;
                }
            }
            else
            {
                
                if (other.gameObject.GetComponent<ValueTrack>() == null) return;
                if (!other.gameObject.GetComponent<ValueTrack>().canActivatePressurePlate) return;
                numberofCurrent++;
                if (!isActivate)
                {
                    foreach (Door door in doors)
                    {
                        door.keysValid++;
                    }
                    
                    meshRenderer.material = onMat;
                    AudioManager.instance.PlayEnvironment("PressurePlate_On");
                    isActivate = true;
                }
            }
        }
        else
        {
            if (other.gameObject.GetComponent<ValueTrack>() == null) return;
            if (!other.gameObject.GetComponent<ValueTrack>().canActivatePressurePlate) return;
            foreach (Door door in doors) { door.keysValid++; }
            
            if (!isActivate)
            {
                AudioManager.instance.PlayEnvironment("PressurePlate_On");
            }
            
            isActivate = true;
            meshRenderer.material = onMat;
        }

        
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player") && !multiActivate)
        {
            numberofCurrent--;
            if (numberofCurrent > 0) return;
            isActivate = false;
            meshRenderer.material = offMat;
            foreach (Door door in doors) { door.keysValid--; }
        }

        if (other.gameObject.GetComponent<ValueTrack>() &&
            other.gameObject.GetComponent<ValueTrack>().canActivatePressurePlate)
        {
            numberofCurrent--;
            if (numberofCurrent > 0) return;
            isActivate = false;
            meshRenderer.material = offMat;
            foreach (Door door in doors) { door.keysValid--; }
        }

        if (isActivate)
        {
            AudioManager.instance.PlayEnvironment("PressurePlate_Off");
        }

    }
}
