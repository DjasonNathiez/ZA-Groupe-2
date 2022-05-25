using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DUNGEON_PuzzleManager : MonoBehaviour
{
    public Door[] puzzle;
    public GameObject[] activationLight;
    public Door finalDoor;

    private void Update()
    {

        if (puzzle[0].puzzleEnded && puzzle[1].puzzleEnded && puzzle[2].puzzleEnded)
        {
            finalDoor.gameObject.SetActive(false);
        }
    }
}
