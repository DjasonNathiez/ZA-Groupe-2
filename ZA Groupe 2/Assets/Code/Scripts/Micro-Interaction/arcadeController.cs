using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class arcadeController : MonoBehaviour
{
    public Rigidbody2D rb;
    public MiniGameManager miniGameManager;
    public int life;
    public float speed = 3;
    [FormerlySerializedAs("m_spriteRenderer")] [SerializeField] private SpriteRenderer spriteRenderer;
    public int game;
    public GameObject projectile;
    public float delayProjectile;
    public float timerProjectile;
    public Vector2 lastMove;
    

    private void Update()
    {

        if (miniGameManager.control)
        {
            switch (game)
            {
                case 0:
                    Vector2 dir = new Vector2(PlayerManager.instance.move.x, PlayerManager.instance.move.z);
                    if (dir != Vector2.zero) lastMove = dir;
                    rb.velocity = dir.normalized * speed;
                    if (dir.x > 0)
                    {
                        spriteRenderer.flipX = false;
                    }
                    else if (dir.x < 0)
                    {
                        spriteRenderer.flipX = true;
                    }

                    if (timerProjectile > 0)
                    {
                        timerProjectile -= Time.deltaTime;
                    }

                    if (PlayerManager.instance.buttonAPressed && timerProjectile <= 0)
                    {
                        PlayerManager.instance.buttonAPressed = false;
                        GameObject newBubble = Instantiate(projectile, transform.position, quaternion.identity,
                            miniGameManager.transform);
                        newBubble.GetComponent<ProjectileArcade>().dir = lastMove;
                        newBubble.GetComponent<ProjectileArcade>().game = 0;
                        newBubble.GetComponent<ProjectileArcade>().miniGameManager = miniGameManager;
                        timerProjectile = delayProjectile;
                    }
                    
                    break;
                
                case 1:
                    Vector2 direction = new Vector2(PlayerManager.instance.move.x, 0);
                    rb.velocity = direction.normalized * speed;
                    break;
                case 4:
                    Vector2 dir4 = new Vector2(PlayerManager.instance.move.x, 0);
                    rb.velocity = new Vector2(dir4.normalized.x * speed,rb.velocity.y);

                    if (dir4.x > 0)
                    {
                        spriteRenderer.flipX = false;
                    }
                    else if (dir4.x < 0)
                    {
                        spriteRenderer.flipX = true;
                    }
                    if (timerProjectile > 0)
                    {
                        timerProjectile -= Time.deltaTime;
                    }

                    if (PlayerManager.instance.buttonAPressed && timerProjectile <= 0)
                    {
                        PlayerManager.instance.buttonAPressed = false;
                        rb.velocity = new Vector2(rb.velocity.x, 13.5f);
                        timerProjectile = delayProjectile;
                    }
                    break;
                case 5:
                    Vector2 dir5 = new Vector2(PlayerManager.instance.move.x, PlayerManager.instance.move.z);
                    rb.velocity = dir5.normalized * speed;

                    if (timerProjectile > 0)
                    {
                        timerProjectile -= Time.deltaTime;
                    }

                    if (PlayerManager.instance.buttonAPressed && timerProjectile <= 0)
                    {
                        PlayerManager.instance.buttonAPressed = false;
                        GameObject newBubble = Instantiate(projectile, transform.position, quaternion.identity,
                            miniGameManager.transform);
                        newBubble.GetComponent<ProjectileArcade>().dir = Vector2.right;
                        newBubble.GetComponent<ProjectileArcade>().game = 5;
                        newBubble.GetComponent<ProjectileArcade>().miniGameManager = miniGameManager;
                        timerProjectile = delayProjectile;
                    }
                    
                    break;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("oui");
        if (other.gameObject.GetComponent<EnnemiArcade>())
        {
            miniGameManager.GameOver();
        }
    }
}
