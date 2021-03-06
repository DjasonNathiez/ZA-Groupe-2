using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Unity.Mathematics;
using UnityEngine;

public class Door : MonoBehaviour
{
    public int keyNeeded;
    public int keysValid;
    public Collider Collider;
    public MeshRenderer MeshRenderer;
    public bool close;
    public bool persistent;
    public string type;
    public Vector3 rotationClosed;
    public Vector3 rotationOpen;
    public GameObject[] lightFeedback;
    public CinematicEnvent cinematicEnvent;
    public bool puzzleEnded;
    public GameObject[] child;
    public bool startedTapeTaupe;
    private bool soundLoad;

    void Update()
    {
        if (keysValid >= keyNeeded)
        {
            if (!soundLoad)
            {
                AudioManager.instance.PlayEnvironment("PuzzleSucces");
                soundLoad = true;
            }

            if (type == "Disabling")
            {
                Collider.enabled = close;
                MeshRenderer.enabled = close;
                if (persistent)
                {
                    enabled = false;
                }   
            }
            else if (type == "Rotating")
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotationOpen), Time.deltaTime * 5);
                if (persistent)
                {
                    keysValid = keyNeeded + 1;
                }   
            }
            else if (type == "Rising")
            {
                transform.position = Vector3.Lerp(transform.position, rotationOpen, Time.deltaTime * 5);
                if (persistent)
                {
                    keysValid = keyNeeded + 10;
                }   
            }
            else if (type == "TapeTaupe")
            {
                if (!startedTapeTaupe)
                {
                    startedTapeTaupe = true;
                    GetComponent<TapeTaupeArcade>().StartTapeTaupe();   
                }
            }
            else if (type == "Dialogue")
            {
                if (!startedTapeTaupe)
                {
                    startedTapeTaupe = true;
                    GetComponent<PnjDialoguesManager>().StartDialogue();   
                }
            }
            else if (type == "ActivateElec")
            {
                GetComponent<ElectrocutedProp>().activated = true;
            }

            if (cinematicEnvent)
            {
                cinematicEnvent.EnableEvent();
            }

            if (type == "Puzzle")
            {
                puzzleEnded = true;
            }
            
            if (type == "Activator")
            {
                foreach (GameObject c in child)
                {
                    c.SetActive(true);
                }
                if (persistent)
                {
                    enabled = false;
                } 
            }
        }
        else
        {
            if (type == "Disabling")
            {
                Collider.enabled = !close;
                MeshRenderer.enabled = !close; 
            }
            else if (type == "Rotating")
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotationClosed), Time.deltaTime * 5);
            }
            else if (type == "Rising")
            {
                transform.position = Vector3.Lerp(transform.position, rotationClosed, Time.deltaTime * 5);
            }
            else if (type == "ActivateElec")
            {
                GetComponent<ElectrocutedProp>().activated = false;
            }

            if (type == "Activator")
            {
                foreach (GameObject c in child)
                {
                    c.SetActive(false);
                }
            }
            else if (type == "TapeTaupe")
            {
                if (startedTapeTaupe)
                {
                    startedTapeTaupe = false;
                }
            }
        }
    }
}
