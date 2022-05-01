using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleEyePillar : MonoBehaviour
{
    public List<GameObject> pillarOrder;
    public List<GameObject> currentPillarTouched;
    public Door door;
    public int pillarOk;
    public bool isActivate;

    private EyeFollow_Advanced eyeFollow;

    private void Start()
    {
        eyeFollow = FindObjectOfType<EyeFollow_Advanced>();
        
        eyeFollow.SwitchFollowedPillar(pillarOrder[0].transform);
    }

    private void Update()
    {
        if (!isActivate)
        {
            CheckPillars();
        }
    }

    private void ResetPillar(GameObject pillar)
    {
        pillar.GetComponent<MeshRenderer>().material.color = Color.grey;
    }

    public void CheckPillars()
    {
        for (int i = 0; i < currentPillarTouched.Count; i++)
        {
            
                if (currentPillarTouched.Count == pillarOrder.Count && !isActivate)
                {
                    if (currentPillarTouched[i] == pillarOrder[i])
                    {
                        pillarOk++;
                    }
                }
                else
                {
                    if (currentPillarTouched[i] == pillarOrder[i])
                    {
                        currentPillarTouched[i].GetComponent<MeshRenderer>().material.color = Color.green;
                        
                        if (eyeFollow)
                        {
                            eyeFollow.SwitchFollowedPillar(pillarOrder[i+1].transform);
                        }
                    }
                    else
                    { 
                        currentPillarTouched[i].GetComponent<MeshRenderer>().material.color = Color.red;
                    }
                }
        }
        
        if (!isActivate && pillarOk == pillarOrder.Count)
        {
            isActivate = true;
            door.keysValid++;
            Debug.Log("Door open with eye pillar");
            gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
        }
    }

    private void ResetPillars()
    {
        foreach (GameObject pi in currentPillarTouched)
        {
            currentPillarTouched.Remove(pi);
        }
    }
}
