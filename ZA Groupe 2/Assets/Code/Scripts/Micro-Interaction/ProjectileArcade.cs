using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileArcade : MonoBehaviour
{
    public Vector2 dir;
    public float speed;
    public int game;
    public MiniGameManager miniGameManager;
    

    void OnTriggerEnter2D(Collider2D other)
    {
        switch (game)
        {
            case 0:
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
                break;
            case 1:
                if (other.gameObject == miniGameManager.player)
                {
                    Destroy(gameObject);
                    miniGameManager.score++;
                    miniGameManager.UpdateScore();
                }
                else if (other.gameObject == miniGameManager.collisions)
                {
                    Destroy(gameObject);
                    miniGameManager.GameOver();
                }
                break;
            case 4:
                Debug.Log(other.name);
                if (other.gameObject == miniGameManager.player)
                {
                    Destroy(gameObject);
                    miniGameManager.score++;
                    miniGameManager.UpdateScore();
                    miniGameManager.SpawnObj();
                }
                break;
            case 5:
                Debug.Log(other.name);
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
                break;
        }
    }

    private void Update()
    {
        switch (game)
        {
            case 0:
                transform.Translate(dir.normalized*speed);
                break;
            case 1:
                transform.Translate(Vector3.down*speed);
                break;
            case 5:
                transform.Translate(Vector3.right*speed);
                break;
        }
    }
}
