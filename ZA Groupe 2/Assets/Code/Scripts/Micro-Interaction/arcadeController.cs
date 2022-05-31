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
                        newBubble.GetComponent<ProjectileOdyskey>().dir = lastMove;
                        newBubble.GetComponent<ProjectileOdyskey>().miniGameManager = miniGameManager;
                        timerProjectile = delayProjectile;
                    }
                    
                    break;
                
                case 1:
                    Vector2 direction = new Vector2(PlayerManager.instance.move.x, 0);
                    rb.velocity = direction.normalized * speed;
                    break;
            }
        }
    }
}
