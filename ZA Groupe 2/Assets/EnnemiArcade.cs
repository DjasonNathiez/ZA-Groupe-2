using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemiArcade : MonoBehaviour
{
    public Transform playerPos;
    public float speed;
    private void Update()
    {
        transform.Translate((playerPos.position - transform.position).normalized * Time.deltaTime * speed);
    }
}
