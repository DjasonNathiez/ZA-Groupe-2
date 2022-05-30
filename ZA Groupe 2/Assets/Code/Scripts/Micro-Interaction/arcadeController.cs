using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
    [FormerlySerializedAs("m_spriteRenderer")] [SerializeField] private SpriteRenderer spriteRenderer;
    public float delay = 2;
    public float spawnTime = 2;
    public float spawnDelay;
    public GameObject ennemi;
    public SpriteRenderer bg;
    public int game;


    private void Start()
    {
        
    }

    private void Update()
    {
        if (onArcade)
        {
            material = screen.GetComponent<MeshRenderer>().material;
            material.color = Color.white;
            if (game == 1)
            {
                title.transform.position = new Vector3(title.transform.position.x, 0, title.transform.position.z);
            }  
            
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

        if (control && game == 0)
        {
            Vector2 dir = new Vector2(PlayerManager.instance.move.x, PlayerManager.instance.move.z);
            rb.velocity = dir.normalized * speed;
            if (dir.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (dir.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            
            
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
        else if (control && game == 1)
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
                        position = new Vector3(Random.Range(242f,258f), -6.5f, -0.2f);
                        break;
                    case 2:
                        position = new Vector3(Random.Range(242f,258f), 6.5f, -0.2f);
                        break;
                    case 3:
                        position = new Vector3(260, Random.Range(-6.5f,6.5f), -0.2f);
                        break;
                    case 4:
                        position = new Vector3(240, Random.Range(-6.5f,6.5f), -0.2f);
                        break;
                }
                Instantiate(ennemi, position, quaternion.identity);
                break;
            }
            case 1:
            {
                
                
                break;
            }
        }
    }
}
