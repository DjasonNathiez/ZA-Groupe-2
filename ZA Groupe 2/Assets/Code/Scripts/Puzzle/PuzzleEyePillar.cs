using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleEyePillar : MonoBehaviour
{
    public List<GameObject> pillarOrder;
    public List<GameObject> currentPillarTouched;
    public int pillarTouchedNumber;
    public Door door;
    public int pillarOk;
    public bool isActivate;

    private EyeFollow_Advanced eyeFollow;

    public List<Material> pillarOrderMaterial;

    private void Start()
    {
        eyeFollow = FindObjectOfType<EyeFollow_Advanced>();
        
        eyeFollow.SwitchFollowedPillar(pillarOrder[0].transform);
    }

    private void Update()
    {
        if (pillarTouchedNumber == 0)
        {
            door.keysValid = 0;
        }

        if (!isActivate)
        {
            CheckPillars();
        }
    }

    public void CheckPillars()
    {
        if (pillarTouchedNumber < pillarOrder.Count)
        {
            eyeFollow.SwitchFollowedPillar(pillarOrder[pillarTouchedNumber].transform);
        }
        
        if (currentPillarTouched.Count != 0)
        {
            if (currentPillarTouched[pillarTouchedNumber-1] == pillarOrder[pillarTouchedNumber-1])
            {
                //Debug.Log("Good");
           
                if (pillarTouchedNumber == pillarOrder.Count)
                {
                    isActivate = true;
                    door.keysValid++;
                    //Debug.Log("Success");
                }
            }
            else
            {
                //Debug.Log("Wrong");
            }
        }
        
    }

    public void ResetPillars()
    {
        foreach (GameObject pi in currentPillarTouched)
        {
            foreach (Material mat in eyeFollow.matObjectX)
            {
                mat.color = pillarOrderMaterial[0].color;
            }
            
            currentPillarTouched.Remove(pi);
            eyeFollow.SwitchFollowedPillar(pillarOrder[0].transform);
        }
    }
}
