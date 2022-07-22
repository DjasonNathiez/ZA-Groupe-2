using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPressurePlate : MonoBehaviour
{
    [SerializeField] private bool isLeftPlate;
    [SerializeField] private float speed;
    [SerializeField] private Material isActivate;
    [SerializeField] private Material isNopeActivate;
    [SerializeField] private MeshRenderer material;
    [SerializeField] private Object[] objects;

    private bool _isMovingRight;
    private bool _isMovingLeft;

    private void Update()
    {
        if (_isMovingLeft) MoveObject("left");
        if (_isMovingRight) MoveObject("right");
    }

    private void MoveObject(string direction)
    {
        switch (direction)
        {
            case "left":
                foreach (Object obj in objects)
                {
                    SetMoveDirection(obj.objectToMove.transform, obj.clampValueLeft);
                    if(obj.inverse) SetMoveDirection(obj.objectToMove.transform, obj.clampValueRight);
                }
                break;
            case "right":
                foreach (Object obj in objects)
                {
                    SetMoveDirection(obj.objectToMove.transform, obj.clampValueRight);
                    if(obj.inverse) SetMoveDirection(obj.objectToMove.transform, obj.clampValueLeft);
                }
                break;
        }
    }

    void SetMoveDirection(Transform obj, Vector3 target)
    {
        obj.position = Vector3.MoveTowards(obj.position, target, speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        switch (isLeftPlate)
        {
            case true:
                _isMovingLeft = true;
                break;
            case false:
                _isMovingRight = true;
                break;
        }
        material.material = isActivate;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        switch (isLeftPlate)
        {
            case true:
                _isMovingLeft = false;
                break;
            case false:
                _isMovingRight = false;
                break;
        }
        material.material = isNopeActivate;
    }
}

[Serializable]
public class Object
{
    public GameObject objectToMove;
    public bool inverse;
    public Vector3 clampValueLeft;
    public Vector3 clampValueRight;
}
