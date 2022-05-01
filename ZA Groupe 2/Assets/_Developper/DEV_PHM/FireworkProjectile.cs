using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireworkProjectile : MonoBehaviour
{
    [SerializeField] private int moveSpeed;
    [SerializeField] private float secondBeforeDestruction;

    private bool canActivate;

    private void Start() { canActivate = true; }
    private void Update()
    {
        if (transform.childCount == 0 || !canActivate) return;
        StartTheFirework();
    }

    private void StartTheFirework()
    {
        LaunchFirework();
        StartCoroutine(GiveRandomDirection());
        StartCoroutine(DestroyFirework());
        canActivate = false;
    }

    private void LaunchFirework()
    {
        Rigidbody rb_firework = GetComponent<Rigidbody>();
        rb_firework.AddForce(transform.forward * moveSpeed, ForceMode.Impulse); 
    }

    IEnumerator GiveRandomDirection()
    {
        while (true)
        {
            Debug.Log("StartRandomDirection");
        
            int rand = Random.Range(1, 4);
            for (int i = 0; i < rand; i++)
            {
                Rigidbody rb_firework = GetComponent<Rigidbody>();
                var forward = transform.forward;
                Vector3 randomVector = Random.insideUnitCircle.normalized;
            
                Vector3 direction = new Vector3(forward.x, randomVector.y, forward.z);

                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
                rb_firework.AddForce(direction * moveSpeed, ForceMode.Impulse);
                rb_firework.velocity = Vector3.ClampMagnitude(rb_firework.velocity, moveSpeed);
            }
            break;
        }
    }
    
    IEnumerator DestroyFirework()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondBeforeDestruction);
            PlayerManager.instance.rope.rewinding = true;
            Destroy(gameObject);
            break;
        }
    }
}
