using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteAlways]
public class Transition_Fog : MonoBehaviour
{
    [SerializeField] private Vector2 baseFogRadiusXHeightY;
    [SerializeField] private Vector2 puzzleFogRadiusXHeightY;
    [SerializeField] private Vector2 baseTransitionRadiusXHeightY;
    [SerializeField] private float etagePos;
    [SerializeField] private float _speed;
    public bool isPuzzle;
    
    private Vector2 fogRadiusXHeightY;
    private Vector2 transitionRadiusXHeightY;
    private float heightPlayer;
    private float durationTime;
    
    //Private float


    private void Start()
    {
        transitionRadiusXHeightY = baseTransitionRadiusXHeightY;
    }

    
    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = transform.localPosition;
        float heightPlayer = playerPosition.y;
        //durationTime += Time.deltaTime;
        //float percentage = durationTime / _speed;

        transitionRadiusXHeightY.y = Mathf.Lerp(baseTransitionRadiusXHeightY.y,10, heightPlayer.Remap(4,8,0,1));
        fogRadiusXHeightY = baseFogRadiusXHeightY;
        
        Shader.SetGlobalVector("_PlayerPosition", playerPosition);
        Shader.SetGlobalFloat("_TransitionRadius", transitionRadiusXHeightY.x);
        Shader.SetGlobalFloat("_TransitionHeight", transitionRadiusXHeightY.y);
        Shader.SetGlobalFloat("_FogRadius", isPuzzle ? Mathf.Lerp(Shader.GetGlobalFloat("_FogRadius"),puzzleFogRadiusXHeightY.x,5*Time.deltaTime) : Mathf.Lerp(Shader.GetGlobalFloat("_FogRadius"),fogRadiusXHeightY.x,5*Time.deltaTime));
        Shader.SetGlobalFloat("_FogHeight", fogRadiusXHeightY.y);
        //Debug.Log(playerPosition.y);
    }
    
    private void OnDisable()
    {
        transitionRadiusXHeightY.x = 200f;
        fogRadiusXHeightY.x = 200f;
    }
}