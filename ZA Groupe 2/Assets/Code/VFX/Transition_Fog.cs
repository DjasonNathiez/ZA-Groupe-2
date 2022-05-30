using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteAlways]
public class Transition_Fog : MonoBehaviour
{
    [SerializeField] private Vector2 baseFogRadiusXHeightY;
    [SerializeField] private Vector2 baseTransitionRadiusXHeightY;
    [SerializeField] private float etagePos;
    [SerializeField] private float _speed;
    
    private Vector2 fogRadiusXHeightY;
    private Vector2 transitionRadiusXHeightY;
    private float heightPlayer;
    private float durationTime;
    
    //Private float


    private void Start()
    {

    }

    
    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = transform.position;
        durationTime += Time.deltaTime;
        float percentage = durationTime / _speed;
        if (playerPosition.y > -15)
        {
            transitionRadiusXHeightY.y = Mathf.Lerp(3,10, percentage);
        }
        fogRadiusXHeightY = baseFogRadiusXHeightY;
        transitionRadiusXHeightY = baseTransitionRadiusXHeightY;
        
        Shader.SetGlobalVector("_PlayerPosition", playerPosition);
        Shader.SetGlobalFloat("_TransitionRadius", transitionRadiusXHeightY.x);
        Shader.SetGlobalFloat("_TransitionHeight", transitionRadiusXHeightY.y);
        Shader.SetGlobalFloat("_FogRadius", fogRadiusXHeightY.x);
        Shader.SetGlobalFloat("_FogHeight", fogRadiusXHeightY.y);
        Debug.Log(transitionRadiusXHeightY.y);
    }
    
    private void OnDisable()
    {
        transitionRadiusXHeightY.x = 200f;
        fogRadiusXHeightY.x = 200f;
    }
}