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
    public float timer;
    public float delay;
    public Transform timerbarre;
    public SpriteRenderer SpriteRenderer;
    

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
            case 2:
                Debug.Log(other.name);
                if (other.gameObject == miniGameManager.player)
                {
                    Destroy(gameObject);
                    miniGameManager.score++;
                    miniGameManager.UpdateScore();
                    miniGameManager.SpawnObj();
                }
                break;
            case 3:
                if (other.gameObject == miniGameManager.collisions)
                {
                    
                    Bounds bounds = other.bounds;
                    float angle = Vector2.SignedAngle(Vector2.right,transform.position - bounds.center);
                    Vector2 rightHigh = (bounds.max - bounds.center);
                    Vector2 rightLow = (new Vector3(bounds.max.x,bounds.min.y,0) - bounds.center);
                    Vector2 leftHigh = (new Vector3(bounds.min.x,bounds.max.y,0) - bounds.center);
                    Vector2 leftLow = (bounds.min - bounds.center);
                    Debug.DrawRay(bounds.center,rightHigh,Color.red,2);
                    Debug.DrawRay(bounds.center,rightLow,Color.green,2);
                    Debug.DrawRay(bounds.center,leftHigh,Color.blue,2);
                    Debug.DrawRay(bounds.center,leftLow,Color.yellow,2);
                    if (angle > Vector2.SignedAngle(Vector2.right, rightLow) &&
                        angle < Vector2.SignedAngle(Vector2.right, rightHigh))
                    {
                        dir = Vector2.Reflect(dir, Vector2.right);
                        if (dir.x > 0) SpriteRenderer.flipX =false;
                        else SpriteRenderer.flipX =true;
                    }
                    else if (angle > Vector2.SignedAngle(Vector2.right, rightHigh) &&
                             angle < Vector2.SignedAngle(Vector2.right, leftHigh))
                    {
                        dir = Vector2.Reflect(dir, Vector2.up);
                        if (dir.x > 0) SpriteRenderer.flipX =false;
                        else SpriteRenderer.flipX =true;
                    }
                    else if (angle > Vector2.SignedAngle(Vector2.right, leftLow) &&
                             angle < Vector2.SignedAngle(Vector2.right, rightLow))
                    {
                        dir = Vector2.Reflect(dir, Vector2.down);
                        if (dir.x > 0) SpriteRenderer.flipX =false;
                        else SpriteRenderer.flipX =true;
                    }
                    else
                    {
                        dir = Vector2.Reflect(dir, Vector2.left);
                        if (dir.x > 0) SpriteRenderer.flipX =false;
                        else SpriteRenderer.flipX =true;
                    }
                }
                else if (other.gameObject == miniGameManager.player)
                {
                    Destroy(gameObject);
                    miniGameManager.score++;
                    miniGameManager.UpdateScore();
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
            case 3:
                if (miniGameManager.control)
                {
                    transform.Translate(dir.normalized*speed);
                    if (timer > 0)
                    {
                        timer -= Time.deltaTime;
                        timerbarre.localScale = new Vector3(timer/delay,1,1);
                    }
                    else
                    {
                        miniGameManager.GameOver();
                    }   
                }
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
