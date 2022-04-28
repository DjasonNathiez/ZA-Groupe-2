using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleEyePillar : MonoBehaviour
{
    public List<GameObject> pillarOrder;
    public List<GameObject> currentPillarTouched;
    public Door door;
    
    public void CheckPillars()
    {
        for (int i = 0; i < currentPillarTouched.Count; i++)
        {
            if (currentPillarTouched.Count != pillarOrder.Count)
            {
                Debug.Log("There is missing pillars. You must have "  + currentPillarTouched.Count + " on " + pillarOrder.Count);
            }
            
            if (currentPillarTouched.Count == pillarOrder.Count)
            {
                if (currentPillarTouched[i] == pillarOrder[i])
                {
                    Debug.Log(currentPillarTouched[i] + " is in the right order");
                    door.keysValid++;
                }
                else
                {
                    Debug.Log(currentPillarTouched[i] + " isn't the right order");
                }
            }
        }
    }
}
