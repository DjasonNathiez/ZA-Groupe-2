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
    public MiniGameManager miniGameManager;
    

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        if (other.GetComponent<EnnemiArcade>())
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
            miniGameManager.score++;
            miniGameManager.UpdateScore();
        }
        else if (other.gameObject == miniGameManager.collisions)
        {
            Destroy(gameObject);   
        }
    }

    private void Update()
    {
        transform.Translate(dir.normalized*speed);
    }
}
