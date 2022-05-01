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
        //StartCoroutine(GiveRandomDirection());
        StartCoroutine(DestroyFirework());
        canActivate = false;
    }

    private void LaunchFirework()
    {
        Rigidbody rbFirework = GetComponent<Rigidbody>();
        rbFirework.AddForce(transform.forward * moveSpeed, ForceMode.Impulse); 
    }

    IEnumerator GiveRandomDirection()
    {
        while (true)
        {
            Debug.Log("StartRandomDirection");
        
            int rand = Random.Range(1, 4);
            for (int i = 0; i < rand; i++)
            {
                Rigidbody rbFirework = GetComponent<Rigidbody>();
                
                var forward = transform.forward;
                Vector3 randomVector = Random.insideUnitCircle.normalized;
            
                Vector3 direction = new Vector3(forward.x, randomVector.y, forward.z);

                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
                rbFirework.AddForce(direction * moveSpeed, ForceMode.Impulse);
                rbFirework.velocity = Vector3.ClampMagnitude(rbFirework.velocity, moveSpeed);
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
