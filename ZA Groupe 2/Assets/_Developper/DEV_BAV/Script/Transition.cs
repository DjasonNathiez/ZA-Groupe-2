using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Transition : MonoBehaviour
{
    [SerializeField] float radius;

    [SerializeField] private Transform playerPos;
    [SerializeField] private Transform etagePos;
    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalVector("_PlayerPosition", transform.position);
        Shader.SetGlobalFloat("_TransitionRadius", radius);
    }
}