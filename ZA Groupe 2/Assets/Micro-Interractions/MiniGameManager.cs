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
    public GameObject ennemi;
    public bool onArcade;
    public GameObject title;
    public bool control;
    public GameObject player;
    public GameObject gameOver;
    public GameObject collisions;
    public float score;
    public TextMeshPro scoreText;
    private void Update()
    {
        if (onArcade)
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
                GameObject newennemi = Instantiate(ennemi, position,quaternion.identity,transform);
                newennemi.GetComponent<EnnemiArcade>().playerPos = player.transform;
                break;
            }
            case 1:
            {
                
                
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

    public void GameOver()
    {
        gameOver.SetActive(true);
        control = false;
        
    }
}
