using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossRocket : MonoBehaviour
{
    public GameObject vfx;
    private Vector3 eulerAngles;
    public Rigidbody rb;
    public Vector3 destination;
    public Vector3 trueDest;
    public float dist;
    public float rotatingSpeed;
    public float velocity;
    public Transform cible;
    public MeshRenderer cibleRender;
    public Material cibleMat;
    public Color cibleColor;
    

    private void Start()
    {
        eulerAngles = transform.eulerAngles;
        rb = GetComponent<Rigidbody>();
        trueDest = destination + (Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.forward).normalized * dist;
        cibleRender = cible.GetComponent<MeshRenderer>();
        cibleMat = new Material(cibleRender.material);
        cibleRender.material = cibleMat;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<BossRocket>())
        {
            GameObject obj = Instantiate(vfx, transform.position + Vector3.down*2, quaternion.identity);
            obj.GetComponent<ParticleSystem>().Play();
            Destroy(obj,3);  
            Destroy(gameObject);   
        }
    }

    private void Update()
    {
        trueDest = destination + (Quaternion.Euler(0, rotatingSpeed, 0) * (trueDest - destination)).normalized * dist;
        transform.rotation = Quaternion.LookRotation((trueDest+Vector3.down*0.54f)-transform.position);
        rb.velocity = transform.forward * velocity;
        Debug.DrawRay(trueDest,Vector3.up);
        cible.position = destination + Vector3.down*0.54f;
        cible.position = new Vector3(cible.position.x, 6.8f, cible.position.z);
        cibleMat.color = Color.Lerp(cibleMat.color, cibleColor, Time.deltaTime * 2);
        cible.rotation = Quaternion.Euler(90,0,0);

    }
}
