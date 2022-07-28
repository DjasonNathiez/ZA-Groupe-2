using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plateformeMove : MonoBehaviour
{
    public Vector3 minimumCoord;
    public Vector3 coordToFollow;
    public Vector3 maximumCoord;
    public rotatingProp[] crankX;
    public rotatingProp[] crankY;
    public rotatingProp[] crankZ;
    public bool playerAboard;
    public float speedX;
    public float speedY;
    public float speedZ;
    public Vector3 lastPos;
    public Rect rect;
    public float yTest;

    private void Start()
    {
        coordToFollow = transform.localPosition;
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + yTest / 2, transform.position.z),
            new Vector3(rect.width, yTest, rect.height));
    }

    public void Update()
    {
        rect.position = new Vector2(transform.position.x - transform.localScale.x / 2,
            transform.position.z - transform.localScale.z / 2);
        if (rect.Contains(new Vector2(PlayerManager.instance.transform.position.x,
                PlayerManager.instance.transform.position.z)) &&
            PlayerManager.instance.transform.position.y > transform.position.y &&
            PlayerManager.instance.transform.position.y < transform.position.y + yTest)
        {
            playerAboard = true;
        }
        else playerAboard = false;

        lastPos = transform.position;
        if (crankX.Length > 0)
        {
            foreach (rotatingProp crank in crankX)
            {
                coordToFollow += new Vector3((crank.myrotation - crank.lastRot) * Time.deltaTime * speedX, 0, 0);
            }
        }

        if (crankY.Length > 0)
        {
            foreach (rotatingProp crank in crankY)
            {
                coordToFollow += new Vector3(0, (crank.myrotation - crank.lastRot) * Time.deltaTime * speedY, 0);
            }
        }

        if (crankZ.Length > 0)
        {
            foreach (rotatingProp crank in crankZ)
            {
                coordToFollow += new Vector3(0, 0, (crank.myrotation - crank.lastRot) * Time.deltaTime * speedZ);
            }
        }

        coordToFollow = new Vector3(
            Mathf.Clamp(coordToFollow.x, minimumCoord.x, maximumCoord.x),
            Mathf.Clamp(coordToFollow.y, minimumCoord.y, maximumCoord.y),
            Mathf.Clamp(coordToFollow.z, minimumCoord.z, maximumCoord.z));

        transform.localPosition = Vector3.Lerp(transform.localPosition, coordToFollow, Time.deltaTime * 5);

        //if (playerAboard) PlayerManager.instance.transform.position += (transform.position - lastPos);
    }
}