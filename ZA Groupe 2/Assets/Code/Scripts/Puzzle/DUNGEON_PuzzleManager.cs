using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DUNGEON_PuzzleManager : MonoBehaviour
{
    public Door[] puzzle;
    public Collider[] activationLight;
    public Door finalDoor;

    private void Update()
    {

        if (activationLight[0].enabled && activationLight[1].enabled && activationLight[2].enabled)
        {
            finalDoor.keysValid = 1;
        }
    }
}
