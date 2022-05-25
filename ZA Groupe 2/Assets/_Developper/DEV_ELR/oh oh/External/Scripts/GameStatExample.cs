using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatExample : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(RadarEventTimer());
    }
    
    IEnumerator RadarEventTimer()
    {
        GameStatsRecorder.Instance.RegisterEvent(new GameStatsLineTemplate(transform.position, "PlayerRadar"));
  

        yield return new WaitForSeconds(1f);

        StartCoroutine(RadarEventTimer());
    }
    void OnJump()
    {
        GameStatsRecorder.Instance.RegisterEvent(new GameStatsLineTemplate()
        {
            EventName = "Jump"
            
        });
    }
}