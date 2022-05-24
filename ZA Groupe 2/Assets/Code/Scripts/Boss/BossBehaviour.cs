using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class BossBehaviour : MonoBehaviour
{
    [FormerlySerializedAs("m_state")] [SerializeField]
    public int state;
    [FormerlySerializedAs("m_phase")] [SerializeField] private int phase;
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float dashingSpeed;
    [FormerlySerializedAs("m_rb")] [SerializeField]
    public Rigidbody rb;
    public Transform legsColider;
    [SerializeField] private Transform[] spawnPosPillars;
    [SerializeField] private Transform[] spawnPosRabbits;
    [SerializeField] private GameObject pillarGameObject;
    [SerializeField] private GameObject shockWaveGameObject;
    [SerializeField] private GameObject rabbitGameObject;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float timeStamp;
    [SerializeField] private float detectionDist;
    [SerializeField] private float shockWaveSpeed = 1;
    [SerializeField] private float shockWaveDuration;
    [SerializeField] private Vector3 dashDirection;
    [SerializeField] public List<GameObject> pillars;
    [SerializeField] private List<GameObject> cableNodes;
    [SerializeField] private float cableRotation;
    [SerializeField] private bossDetector bossDetector;
    [SerializeField] private Material material;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Animator animator;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        timeStamp = timeBetweenAttacks;
        material = new Material(material);
        GetComponent<MeshRenderer>().material = material;
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
                bossDetector.transform.rotation = Quaternion.Lerp(bossDetector.transform.rotation,Quaternion.LookRotation(forward),Time.deltaTime*rotationSpeed);
                if (cableRotation >= 360 || cableRotation <= -360)
                {
                    PlayerManager.instance.rope.rewinding = true;
                    state = 0;
                    StartCoroutine(Fall(1));
                    Debug.Log("Falls");
                }
                break;
            case 2:
                rb.velocity = dashDirection * dashingSpeed;
                bossDetector.transform.rotation = Quaternion.LookRotation(dashDirection);
                break;
        }

        if (timeStamp <= 0 )
        {
            timeStamp = timeBetweenAttacks;
            if (pillars.Count == 0)
            {
                state = 0;
                StartCoroutine(SpawnRabbits(1));
            }
            else
            {
                if (Vector3.SqrMagnitude(PlayerManager.instance.transform.position - transform.position) < detectionDist * detectionDist)
                {
                    state = 0;
                    StartCoroutine(ShockWave(1));
                }
                else
                {
                    state = 0;
                    StartCoroutine(Dash(0.2f));
                }
            }
        }

        if (shockWaveGameObject.activeSelf)
        {
            shockWaveGameObject.transform.localScale += new Vector3((Time.deltaTime * shockWaveSpeed),0,(Time.deltaTime * shockWaveSpeed));
        }

        if (legsColider.childCount > 0 + cableNodes.Count || legsColider.childCount < 1 + cableNodes.Count)
        {
            List<GameObject> nodes = new List<GameObject>(0);
            foreach (Node node in PlayerManager.instance.rope.nodes)
            {
                if (node.anchor == legsColider.gameObject)
                {
                    nodes.Add(node.nodePoint);
                }
            }
            cableNodes = nodes;
            cableRotation = CalculateCableRotation();
        }
    }

    public IEnumerator SpawnRabbits(float delay)
    {
        material.color = Color.cyan;
        yield return new WaitForSeconds(delay);
        foreach (Transform pos in spawnPosPillars)
        {
            GameObject pillar = Instantiate(pillarGameObject, pos.position, quaternion.identity);
            pillars.Add(pillar);
        }
        foreach (Transform pos in spawnPosRabbits)
        {
            GameObject rabbit = Instantiate(rabbitGameObject, pos.position, quaternion.identity);
            rabbit.SetActive(true);
        }
        material.color = Color.white;
        state = 1;
    }

    public float CalculateCableRotation()
    {
        float cableRot = 0;
        if (cableNodes.Count > 1)
        {
            Debug.Log("okBienOUej");
            for (int i = 1; i < cableNodes.Count; i++)
            {
                Vector3 pos = cableNodes[i - 1].transform.position;
                Vector3 nextpos = cableNodes[i].transform.position;
                cableRot += Vector3.SignedAngle(new Vector3(pos.x,transform.position.y,pos.z) - transform.position, new Vector3(nextpos.x,transform.position.y,nextpos.z) - transform.position, Vector3.up);
            }   
        }
        return cableRot;
    }
    
    public IEnumerator Dash(float delay)
    {
        animator.Play("Dash");
        material.color = Color.cyan;
        yield return new WaitForSeconds(delay);
        dashDirection = (PlayerManager.instance.transform.position - transform.position).normalized;
        dashDirection = new Vector3(dashDirection.x, 0, dashDirection.z);
        transform.rotation = Quaternion.LookRotation(dashDirection);
        material.color = Color.blue;
        state = 2;
    }
    
    public IEnumerator ReturnToIddle(float delay)
    {
        material.color = Color.cyan;
        yield return new WaitForSeconds(delay);
        material.color = Color.white;
        state = 1;
        animator.Play("Marche");
    }
    
    public IEnumerator ShockWave(float delay)
    {
        material.color = Color.cyan;
        animator.Play("Attaque");
        yield return new WaitForSeconds(delay);
        shockWaveGameObject.SetActive(true);
        shockWaveGameObject.transform.position = transform.position - new Vector3(0,2,0);
        shockWaveGameObject.transform.localScale = new Vector3(2, 0.2f, 2);
        material.color = Color.blue;
        yield return new WaitForSeconds(shockWaveDuration);
        material.color = Color.white;
        shockWaveGameObject.SetActive(false);
        state = 1;
        animator.Play("Marche");
    }

    public IEnumerator Fall(float delay)
    {
        state = 0;
        material.color = Color.yellow;
        yield return new WaitForSeconds(delay);
        material.color = Color.red;
        state = 3;
    }
    
    
}