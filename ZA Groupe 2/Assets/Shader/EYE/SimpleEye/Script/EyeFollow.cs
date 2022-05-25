using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteAlways]
public class EyeFollow : MonoBehaviour
{
    //ObjectToFollow
    public Transform positionToFollow;
    //List Material
    public List<Material> eyeMat;
    public List<Material> eyeMatInvertX;

    //private
    private Vector3 _positionObject;
    private float _positionObjectX;
    private float _positionObjectY;
    private float _positionObjectZ;

    private float eyeMatPosition;
    private float eyeMatInvertXPosition;
    
    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        UpdateMaterial();
    }

    void UpdatePosition()
    {
        _positionObject = positionToFollow.position;
        _positionObjectX = _positionObject.x;
        _positionObjectY = _positionObject.y;
        _positionObjectZ = _positionObject.z;
        
        _positionObjectX = _positionObjectX switch
        {
            > 0.5f => 0.5f,
            < -0.5f => -0.5f,
            _ => _positionObjectX
        };
            
        _positionObjectY = _positionObjectY switch
        {
            > 0.5f => 0.5f,
            < -0.5f => -0.5f,
            _ => _positionObjectY
        };
        
        _positionObjectZ = _positionObjectZ switch
        {
            > 0.5f => 0.5f,
            < -0.5f => -0.5f,
            _ => _positionObjectZ
        };
        

    }
    

    void UpdateMaterial()
    {
        foreach (Material eye in eyeMat)
        {
            eye.SetFloat("_Eye_Position_X_Outside", _positionObjectZ * -_positionObjectX);
            eye.SetFloat("_Eye_Position_Y_Outside", _positionObjectZ * _positionObjectY);
        }

        foreach (Material eye in eyeMatInvertX)
        {
            eye.SetFloat("_Eye_Position_X_Outside", _positionObjectX * _positionObjectZ);
            eye.SetFloat("_Eye_Position_Y_Outside", _positionObjectX * _positionObjectY);
        }
    }

    /*
    void EyeAlignement()
    {
        eyeOffset = new Vector2(transform.localPosition.x, -transform.localPosition.y);
        // clamp so the eye doesnt disappear  
        if(eyeOffset.x < -eyeMaxOffset || eyeOffset.x > eyeMaxOffset)
        {
            eyeOffset.x = Mathf.Clamp(eyeOffset.x, -eyeMaxOffset, eyeMaxOffset);
        }
        if(eyeOffset.y < -eyeMaxOffset || eyeOffset.y > eyeMaxOffset)
        {
            eyeOffset.y = Mathf.Clamp(eyeOffset.y, -eyeMaxOffset, eyeMaxOffset);
        }
    }
    */
}