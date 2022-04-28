using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleBoutton : MonoBehaviour
{
    public bool isGripped;
    public bool isActivated;
    public Door door;
    
    [Header("Enigme Donjon")] 
    public bool isEnigmeLustre;
    public Rigidbody lustreRb;
    
    private void Update()
    {
        if (isEnigmeLustre && isGripped)
        {
            lustreRb.isKinematic = false;
        }

        if (isGripped && !isActivated)
        {
            isActivated = true;
            door.keysValid++;
        }
    }
}
