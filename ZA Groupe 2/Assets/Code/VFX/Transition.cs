using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways ]
public class Transition : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] Vector3 radiusDebug;
    
    [Header("Transition Etage")]
    [SerializeField] private Transform etagePos;
    [SerializeField] private float heightPlayer;
    [SerializeField] private float _duration;
    [SerializeField]
    
    //Private float
    private float _speed;

    private void Start()
    {
        radius= 0f;
    }

    /*
    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = PlayerManager.instance.transform.position;
        Shader.SetGlobalVector("_PlayerPosition", playerPosition);
        heightPlayer =  (etagePos.localPosition.y + 1) - playerPosition.y;
        //Debug.Log(heightPlayer);
        if (heightPlayer != 0)
        {
            if (heightPlayer >= 0)
            {
                radius = Mathf.Lerp(0, 80, heightPlayer.Remap(4, 2, 0, 1));
            }
            Shader.SetGlobalFloat("_TransitionRadius", radius);
        }
    }
    */

    void Update()
    {
        Shader.SetGlobalVector("_PlayerPosition_2", transform.position);
        Shader.SetGlobalVector("_TransitionDebug_1", radiusDebug);
        Shader.SetGlobalFloat("_TransitionRadiu", radius);
    }

    private void OnDisable()
    {
        radius = 200f;
        Shader.SetGlobalVector("_TransitionDebug_1", radiusDebug);
        Shader.SetGlobalFloat("_TransitionRadiu", radius);
    }
}