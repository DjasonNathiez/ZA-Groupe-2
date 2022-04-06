using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class arcadeController : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject title;
    public GameObject screen;
    public Material material;
    public bool onArcade;
    public bool control;
    public int life;
    public float speed = 3;
    [SerializeField] private SpriteRenderer m_spriteRenderer;
    public float delay = 2;


    private void Start()
    {
        material = screen.GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        if (onArcade)
        {
            material.color = Color.white;
            
            if (delay <= 0)
            {
                control = true;
                title.SetActive(false);
            }
            else
            {
                delay -= Time.deltaTime;
            }
        }

        if (control)
        {
            Vector2 dir = new Vector2(PlayerManager.instance.move.x, PlayerManager.instance.move.z);
            rb.velocity = dir.normalized * speed;
            if (dir.x > 0)
            {
                m_spriteRenderer.flipX = false;
            }
            else if (dir.x < 0)
            {
                m_spriteRenderer.flipX = true;
            }
        }
    }
}
