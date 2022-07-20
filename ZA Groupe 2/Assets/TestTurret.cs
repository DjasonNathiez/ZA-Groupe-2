using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Unity.Mathematics;
using UnityEngine;

public class TestTurret : MonoBehaviour
{
    public float speed;
    public GameObject ball;
    public float timer;
    public float delay;
    public Vector3 velocity;

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            GameObject newBall = Instantiate(ball, transform.position, quaternion.identity);
            newBall.GetComponent<bulletBehavior>().speed = speed;
            newBall.GetComponent<bulletBehavior>().velocity = velocity;
            timer = delay;
        }
    }
}
