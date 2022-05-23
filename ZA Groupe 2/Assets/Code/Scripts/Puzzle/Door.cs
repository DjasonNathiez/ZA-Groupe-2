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
    public bool puzzleEnded;
    
    void Update()
    {
        if (keysValid >= keyNeeded)
        {
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

            if (type == "Puzzle")
            {
                puzzleEnded = true;
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
        }
    }
}
