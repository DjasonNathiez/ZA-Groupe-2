using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways ]
public class Transition_Fog : MonoBehaviour
{
    [SerializeField] Vector2 fogRadiusXHeightY;
    [SerializeField] Vector2 transitionRadiusXHeightY;
    
    
    //Private float
    private float _speed;

    private void Start()
    {
        fogRadiusXHeightY = new Vector2(0f,0f);
        transitionRadiusXHeightY = new Vector2(0f,0f);
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
        Shader.SetGlobalFloat("TransitionRadius", transitionRadiusXHeightY.x);
        Shader.SetGlobalFloat("TransitionHeight", transitionRadiusXHeightY.y);
        Shader.SetGlobalFloat("FogRadius", fogRadiusXHeightY.x);
        Shader.SetGlobalFloat("FogHeight", fogRadiusXHeightY.y);
    }

    private void OnDisable()
    {
        transitionRadiusXHeightY.x = 200f;
        fogRadiusXHeightY.x = 200f;
    }
}