using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MiniGameManager : MonoBehaviour
{
    public float delay = 2;
    public float spawnTime = 2;
    public float spawnDelay;
    public int game;
    public GameObject[] ennemi;
    public bool onArcade;
    public GameObject title;
    public bool control;
    public GameObject player;
    public GameObject gameOver;
    public GameObject collisions;
    public float score;
    public TextMeshPro scoreText;
    public bool endGame;
    public CameraController cam;
    public Vector3 PlayerStartPos;
    public Material material;
    public MeshRenderer screenMaterial;
    public Vector3[] spawnPoints;
    public int previousSpawn;
    public GameObject obj;

    private void Start()
    {
        screenMaterial.material = new Material(screenMaterial.material);
    }

    private void Update()
    {
        if (onArcade && !endGame)
        {
            if (delay <= 0)
            {
                PlayerManager.instance.buttonAPressed = false;
                title.SetActive(false);
                control = true;
                onArcade = false;
            }
            else
            {
                delay -= Time.deltaTime;
            }

            scoreText.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), Mathf.Clamp(delay, 0, 1));
        }
        else if (onArcade && endGame)
        {
            if (delay <= 0)
            {
                title.SetActive(true);
                onArcade = false;
                delay = 3.5f;
                score = 0;
                UpdateScore();
                foreach (EnnemiArcade ennemi in transform.GetComponentsInChildren<EnnemiArcade>())
                {
                    Destroy(ennemi.gameObject);
                }
                cam.playerFocused = true;
                cam.cameraPos.position  = transform.position;
                cam.cameraPos.rotation = Quaternion.Euler(45,-45,0);
                cam.cameraZoom = 8.22f;
                endGame = false;
                player.transform.position = PlayerStartPos;
                scoreText.color = new Color(1, 1, 1, 0);
                screenMaterial.material.color = Color.black;
                gameOver.SetActive(false);
                PlayerManager.instance.ExitDialogue();
            }
            else
            {
                delay -= Time.deltaTime;
            }
        }

        if (control)
        {
            if (spawnDelay <= 0)
            {
                spawnDelay = spawnTime;
                SpawnEnnemi();
            }
            else
            {
                spawnDelay -= Time.deltaTime;
            }
        }
        if (game == 5)
        {
            material.SetTextureOffset("_MainTex",new Vector2(Time.time/5,0));
        }
    }
    private void SpawnEnnemi()
    {
        switch (game)
        {
            case 0:
            {
                int rng = Random.Range(1, 5);
                Vector3 position = default;
                switch (rng)
                {
                    case 1:
                        position = transform.position + new Vector3(Random.Range(-8f,8f), -6.5f, -0.2f);
                        break;
                    case 2:
                        position = transform.position + new Vector3(Random.Range(-8f,8f), 6.5f, -0.2f);
                        break;
                    case 3:
                        position = transform.position + new Vector3(10, Random.Range(-6.5f,6.5f), -0.2f);
                        break;
                    case 4:
                        position = transform.position + new Vector3(-10, Random.Range(-6.5f,6.5f), -0.2f);
                        break;
                }
                GameObject newennemi = Instantiate(ennemi[0], position,quaternion.identity,transform);
                newennemi.GetComponent<EnnemiArcade>().playerPos = player.transform;
                break;
            }
            case 1:
            {
                Vector3 position = transform.position + new Vector3(Random.Range(-7f,7f), 6.5f, -0.2f);
                GameObject newFood = Instantiate(ennemi[Random.Range(0,2)], position,quaternion.identity,transform);
                newFood.GetComponent<ProjectileArcade>().game = 1;
                newFood.GetComponent<ProjectileArcade>().miniGameManager = this;
                
                break;
            }
            case 4:
            {
                int rng = Random.Range(1, 7);
                Vector3 position = default;
                switch (rng)
                {
                    case 1:
                        position = transform.position + new Vector3(8.8f,3.1f,-0.2f);
                        break;
                    case 2:
                        position = transform.position + new Vector3(8.8f,0.5f,-0.2f);
                        break;
                    case 3:
                        position = transform.position + new Vector3(8.8f,-2.5f,-0.2f);
                        break;
                    case 4:
                        position = transform.position + new Vector3(-8.8f,3.1f,-0.2f);
                        break;
                    case 5:
                        position = transform.position + new Vector3(-8.8f,0.5f,-0.2f);
                        break;
                    case 6:
                        position = transform.position + new Vector3(-8.8f,-2.5f,-0.2f);
                        break;
                }
                GameObject newennemi = Instantiate(ennemi[0], position,quaternion.identity,transform);
                
                if(rng <=3) newennemi.GetComponent<EnnemiArcade>().speed = 3;
                else
                {
                    newennemi.GetComponent<EnnemiArcade>().speed = -3;
                    newennemi.GetComponent<SpriteRenderer>().flipX = true;
                }
                break;
            }
            case 5:
            {
                Vector3 position = transform.position + new Vector3(10, Random.Range(-6f,6f), -0.2f);
                GameObject newennemi = Instantiate(ennemi[0], position,quaternion.identity,transform);
                newennemi.GetComponent<EnnemiArcade>().playerPos = player.transform;
                
                break;
            }
        }
    }

    public void UpdateScore()
    {
        if (score < 10)
        {
            scoreText.text = "00" +score;
        }
        else if (score >= 10 && score < 100)
        {
            scoreText.text = "0" +score;
        }
        else
        {
            scoreText.text = score.ToString();
        }
    }

    public void SpawnObj()
    {
        switch (game)
        {
            case 4:
                int choose = Random.Range(0, spawnPoints.Length);
                if (choose == previousSpawn)
                {
                    choose = (choose + 1) % spawnPoints.Length;
                }
                previousSpawn = choose;
                GameObject newObj = Instantiate(obj, spawnPoints[choose],quaternion.identity, transform);
                newObj.GetComponent<ProjectileArcade>().miniGameManager = this;
                break;
        }
    }

    public void GameOver()
    {
        gameOver.SetActive(true);
        control = false;
        onArcade = true;
        delay = 3;
        endGame = true;
    }
}
