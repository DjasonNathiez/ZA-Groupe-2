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
        if (puzzle[0].puzzleEnded)
        {
            activationLight[0].SetActive(true);
        }

        if (puzzle[1].puzzleEnded)
        {
            activationLight[1].SetActive(true);
        }

        if (puzzle[2].puzzleEnded)
        {
            activationLight[2].SetActive(true);
        }

        if (puzzle[0].puzzleEnded && puzzle[1].puzzleEnded && puzzle[2].puzzleEnded)
        {
            finalDoor.gameObject.SetActive(false);
        }
    }
}
