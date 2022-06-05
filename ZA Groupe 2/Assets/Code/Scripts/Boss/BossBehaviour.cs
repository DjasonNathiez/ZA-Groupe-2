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
    [SerializeField] private List<Pilone> pilones;
    [SerializeField] private bossDetector bossDetector;
    [SerializeField] private Material material;
    [SerializeField] private float rotationSpeed;
    public Animator animator;
    public GameObject[] vfx;
    public Transform attackSpawnPlace;
    public Transform DeathSpawnPlace;

    
    [Header("VFX")]
    public ParticleSystem hurtVFX;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        timeStamp = timeBetweenAttacks;
        material = new Material(material);
        GetComponent<MeshRenderer>().material = material;
        animator.Play("Marche");
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
                if (cableRotation is >= 360 or <= -360)
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
            case 4:
                if (cableRotation is >= 360 or <= -360)
                {
                    PlayerManager.instance.rope.rewinding = true;
                    state = 0;
                    StartCoroutine(Fall(1));
                    Debug.Log("Falls");
                }
                break;
        }

        if (timeStamp <= 0 )
        {
            timeStamp = timeBetweenAttacks;
            if (pillars.Count == 0)
            {
                state = 0;
                StartCoroutine(SpawnRabbits(2.2f,1.20f));
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

        if (legsColider.childCount > 1 + cableNodes.Count || legsColider.childCount < 1 + cableNodes.Count)
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
            if (cableNodes.Count > 0 && phase == 0 && state == 1)
            {
                state = 4;
                animator.Play("BeforeFalling");
            }
            else if (cableNodes.Count == 0 && phase == 0 && state == 4)
            {
                state = 1;
                animator.Play("Marche");
            }
            cableRotation = CalculateCableRotation();
        }
    }

    public IEnumerator SpawnRabbits(float delay,float vfxDelay)
    {
        material.color = Color.cyan;
        animator.Play("Lance-Lapin");
        yield return new WaitForSeconds(vfxDelay);
        vfx[1].SetActive(true);
        vfx[1].GetComponent<ParticleSystem>().Stop();
        vfx[1].GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(delay-vfxDelay);
        
        foreach (Transform pos in spawnPosPillars)
        {
            GameObject pillar = Instantiate(pillarGameObject, pos.position, quaternion.identity);
            GameObject vfxpillar = Instantiate(vfx[2], pos.position, quaternion.identity);
            vfxpillar.transform.rotation = Quaternion.Euler(-90,0,0);
            Destroy(vfxpillar,3);
            pillars.Add(pillar);
        }
        foreach (Transform pos in spawnPosRabbits)
        {
            GameObject rabbit = Instantiate(rabbitGameObject, pos.position, quaternion.identity);
            rabbit.SetActive(true);
        }
        material.color = Color.white;
        state = 1;
        animator.Play("Marche");
    }

    public float CalculateCableRotation()
    {
        float cableRot = 0;
        if (cableNodes.Count > 1)
        {
            for (int i = 1; i < cableNodes.Count; i++)
            {
                Vector3 pos = cableNodes[i - 1].transform.position;
                Vector3 nextpos = cableNodes[i].transform.position;
                cableRot += Vector3.SignedAngle(new Vector3(pos.x,transform.position.y,pos.z) - transform.position, new Vector3(nextpos.x,transform.position.y,nextpos.z) - transform.position, Vector3.up);
            }   
        }
        
        foreach (var pilone in pilones)
        {
            if (cableRot > pilone.rotation || cableRot < -pilone.rotation) pilone.pilone.SetActive(true);
            else pilone.pilone.SetActive(false);
        }

        return cableRot;
    }
    
    public IEnumerator Dash(float delay)
    {
        animator.Play("Dash");
        material.color = Color.cyan;
        yield return new WaitForSeconds(delay);
        vfx[3].SetActive(true);
        vfx[3].GetComponent<ParticleSystem>().Stop();
        vfx[3].GetComponent<ParticleSystem>().Play();
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
        shockWaveGameObject.transform.position = transform.position - new Vector3(0,2,0) + bossDetector.transform.forward*2;
        shockWaveGameObject.transform.localScale = new Vector3(2, 1, 2);
        material.color = Color.blue;
        GameObject attack = Instantiate(vfx[6], transform.position - new Vector3(0,0.35f,0) + bossDetector.transform.forward*2,quaternion.identity,attackSpawnPlace);
        attack.transform.SetParent(null);
        attack.transform.rotation = Quaternion.Euler(-90,0,0);
        yield return new WaitForSeconds(shockWaveDuration);
        
        material.color = Color.white;
        shockWaveGameObject.SetActive(false);
        state = 1;
        animator.Play("Marche");
    }

    public IEnumerator Fall(float delay)
    {
        Debug.Log("Chute");
        animator.Play("Chute");
        rb.isKinematic = true;
        state = 0;
        material.color = Color.yellow;
        yield return new WaitForSeconds(delay);
        
        material.color = Color.red;
        state = 3;
    }
    
    public IEnumerator StandUp(float delay)
    {
        rb.isKinematic = false;
        animator.Play("StandUp");
        yield return new WaitForSeconds(delay);
        foreach (GameObject obj in pillars)
        {
            Destroy(obj);
        }
        pillars.Clear();
        StartCoroutine(SpawnRabbits(2.2f,1.2f));
    }
    public void GetHurt(int damage)
    {
        if (state == 3)
        {
            if (hurtVFX != null)
            {
                hurtVFX.Play();
            }

            if (phase < 2)
            {
                state = 0;
                StartCoroutine(StandUp(2));
                phase++;
            }
            else
            {
                state = 0;
                animator.Play("Mort");
                GameObject death = Instantiate(vfx[5], DeathSpawnPlace);
            }
        }
    }
}

[Serializable]
public class Pilone
{
    public GameObject pilone;
    public float rotation;
}
