using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileOdyskey : MonoBehaviour
{
    public Vector2 dir;
    public float speed;

    void OnTriggerEnter()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        transform.Translate(dir.normalized*speed);
    }
}
