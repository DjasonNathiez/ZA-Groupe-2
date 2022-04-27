using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class BossBehaviour : MonoBehaviour
{
    [FormerlySerializedAs("m_state")] [SerializeField] private int state;
    [FormerlySerializedAs("m_phase")] [SerializeField] private int phase;
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float dashingSpeed;
    [FormerlySerializedAs("m_rb")] [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector3[] spawnPosPillars;
    [SerializeField] private Vector3[] spawnPosRabbits;
    [SerializeField] private GameObject pillarGameObject;
    [SerializeField] private GameObject shockWaveGameObject;
    [SerializeField] private GameObject rabbitGameObject;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float timeStamp;
    [SerializeField] private float detectionDist;
    [SerializeField] private float shockWaveSpeed = 1;
    [SerializeField] private float shockWaveDuration;
    [SerializeField] private Vector3 dashDirection;
    [SerializeField] private List<GameObject> pillars;
    [SerializeField] private float cableFirstAngle;
    [SerializeField] private float cableLastAngle;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        timeStamp = timeBetweenAttacks;
    }

    void Update()
    {
        switch (state)
        {
            case 0:
                break;
            case 1:
                Vector3 forward = (PlayerManager.instance.transform.position - transform.position);
                timeStamp -= Time.deltaTime;
                forward = new Vector3(forward.x, 0, forward.z).normalized * walkingSpeed;
                rb.velocity = forward;
                transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(forward),Time.deltaTime*5);
                break;
            case 2:
                rb.velocity = dashDirection * dashingSpeed;
                break;
        }

        if (timeStamp <= 0 )
        {
            timeStamp = timeBetweenAttacks;
            if (pillars.Count == 0)
            {
                state = 0;
                StartCoroutine(SpawnRabbits(1));
                Debug.Log("Rabbits");
            }
            else
            {
                if (Vector3.SqrMagnitude(PlayerManager.instance.transform.position - transform.position) < detectionDist * detectionDist)
                {
                    state = 0;
                    StartCoroutine(ShockWave(1));
                    Debug.Log("ShockWave");
                }
                else
                {
                    state = 0;
                    StartCoroutine(Dash(1));
                    Debug.Log("Dash");
                }
            }
        }

        if (cableLastAngle > cableFirstAngle + 360 || cableLastAngle < cableFirstAngle - 360)
        {
            state = 0;
            StartCoroutine(Fall(1));
            Debug.Log("Falls");
        }

        if (shockWaveGameObject.activeSelf)
        {
            shockWaveGameObject.transform.localScale += new Vector3((Time.deltaTime * shockWaveSpeed),0,(Time.deltaTime * shockWaveSpeed));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state == 2)
        {
            if (other.gameObject.CompareTag("UngrippableObject"))
            {
                state = 0;
                rb.velocity = Vector3.zero;
                StartCoroutine(ReturnToIddle(1));
            }
            else if (pillars.Contains(other.gameObject))
            {
                state = 0;
                pillars.Remove(other.gameObject);
                Destroy(other.gameObject);
                rb.velocity = Vector3.zero;
                StartCoroutine(ReturnToIddle(1));
            }
            else if (other.gameObject.CompareTag("Player"))
            {
                state = 0;
                rb.velocity = Vector3.zero;
                
                // Faire perdre 1 point de vie au joueur ------------------------
                
                StartCoroutine(ReturnToIddle(1));
            }
        }
    }

    public IEnumerator SpawnRabbits(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (Vector3 pos in spawnPosPillars)
        {
            GameObject pillar = Instantiate(pillarGameObject, pos, quaternion.identity);
            pillars.Add(pillar);
        }
        state = 1;
    }
    
    public IEnumerator Dash(float delay)
    {
        yield return new WaitForSeconds(delay);
        dashDirection = (PlayerManager.instance.transform.position - transform.position).normalized;
        dashDirection = new Vector3(dashDirection.x, 0, dashDirection.z);
        transform.rotation = Quaternion.LookRotation(dashDirection);
        state = 2;
    }
    
    public IEnumerator ReturnToIddle(float delay)
    {
        yield return new WaitForSeconds(delay);
        state = 1;
    }
    
    public IEnumerator ShockWave(float delay)
    {
        yield return new WaitForSeconds(delay);
        shockWaveGameObject.SetActive(true);
        shockWaveGameObject.transform.position = transform.position - new Vector3(0,2,0);
        shockWaveGameObject.transform.localScale = new Vector3(2, 0.2f, 2);
        yield return new WaitForSeconds(shockWaveDuration);
        shockWaveGameObject.SetActive(false);
        state = 1;
    }

    public IEnumerator Fall(float delay)
    {
        state = 0;
        yield return new WaitForSeconds(delay);
        state = 3;
    }
    
    
}
