using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemiArcade : MonoBehaviour
{
    public Transform playerPos;
    public int game;
    public Vector2 dir;
    public float speed;
    public SpriteRenderer spriteRenderer;
    private void Update()
    {
        switch (game)
        {
            case 0:
                transform.Translate((playerPos.position - transform.position).normalized * Time.deltaTime * speed);
                if ((playerPos.position - transform.position).x > 0) spriteRenderer.flipX = true;
                else spriteRenderer.flipX = false;
                break;
            case 2:
                transform.Translate(dir.normalized * Time.deltaTime * speed);
                if (dir.x > 0) spriteRenderer.flipX = true;
                else spriteRenderer.flipX = false;
                if(transform.position.x < transform.parent.position.x -10 || transform.position.x > transform.parent.position.x +10 || transform.position.y < transform.parent.position.y -8 || transform.position.y > transform.parent.position.y +8 ) Destroy(gameObject);

                break;
            case 4:
                transform.Translate(Vector3.left * Time.deltaTime * speed);
                if(transform.position.x < transform.parent.position.x -10 || transform.position.x > transform.parent.position.x +10 ) Destroy(gameObject);
                break;
            case 5:
                transform.Translate(new Vector3(-1,(playerPos.position - transform.position).normalized.y,0) * Time.deltaTime * speed);
                if(transform.position.x < transform.parent.position.x -10) Destroy(gameObject);
                break;
        }
    }
}
