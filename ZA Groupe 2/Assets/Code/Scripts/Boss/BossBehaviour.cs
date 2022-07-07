using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BossBehaviour : MonoBehaviour
{
    [FormerlySerializedAs("m_state")] [SerializeField]
    public int state;
    [FormerlySerializedAs("m_phase")] [SerializeField] private int phase;
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float walkingSpeed3;
    [SerializeField] private float dashingSpeed;
    [FormerlySerializedAs("m_rb")] [SerializeField]
    public Rigidbody rb;
    public Transform legsColider;
    [SerializeField] private GameObject[] spawnPosPillars;
    [SerializeField] private Transform[] spawnPosRabbits;
    [SerializeField] private GameObject pillarGameObject;
    [SerializeField] private GameObject shockWaveGameObject;
    [SerializeField] private GameObject rabbitGameObject;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float timeBetweenAttackPhaseThree;
    [SerializeField] private float timeStamp;
    [SerializeField] private float detectionDist;
    [SerializeField] private float shockWaveSpeed = 1;
    [SerializeField] private float shockWaveSpeed3 = 1;
    [SerializeField] private float shockWaveDuration;
    [SerializeField] private float shockWaveDuration2;
    [SerializeField] private Vector3 dashDirection;
    [SerializeField] public List<GameObject> pillars;
    [SerializeField] private List<GameObject> cableNodes;
    [SerializeField] private float cableRotation;
    [SerializeField] private List<Pilone> pilones;
    [SerializeField] private bossDetector bossDetector;
    [SerializeField] private Material material;
    [SerializeField] private float rotationSpeed;
    public bool spawnedRabbit;
    public Animator animator;
    public GameObject[] vfx;
    public Transform attackSpawnPlace;
    public Transform DeathSpawnPlace;
    public AnimationCurve animationHurt;
    public bool hurtAnim;
    public float hurtTime;
    public Material aggroMaterial;
    public List<SkinnedMeshRenderer> modelMeshRenderer;
    public List<MeshRenderer> modelMeshRenderer2;
    public Teleporter teleporter;
    public Transform afterBossPos;
    public PnjDialoguesManager pnjDialoguesManager;
    public BossRollerCoaster BossRollerCoaster;

    
    [Header("VFX")]
    public ParticleSystem hurtVFX;
    
    [Header("Temp")]
    [SerializeField] private FollowCurve followCurve;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        timeStamp = timeBetweenAttacks;
        material = new Material(material);
        GetComponent<MeshRenderer>().material = material;
        animator.Play("Marche");
        aggroMaterial = new Material(aggroMaterial);
        foreach (SkinnedMeshRenderer mesh in modelMeshRenderer)
        {
            mesh.material = aggroMaterial;
        }
        foreach (MeshRenderer mesh in modelMeshRenderer2)
        {
            mesh.material = aggroMaterial;
        }
        
    }

    void Update()
    {
        switch (state)
        {
            case 0:
                break;
            case 1:
                Vector3 forward;
                if(!followCurve.moving) forward = (PlayerManager.instance.transform.position - transform.position);
                else forward = (followCurve.transform.position - transform.position);
                timeStamp -= Time.deltaTime;
                forward = new Vector3(forward.x, 0, forward.z).normalized * walkingSpeed;
                rb.velocity = forward;
                bossDetector.transform.rotation = Quaternion.Lerp(bossDetector.transform.rotation,Quaternion.LookRotation(forward),Time.deltaTime*rotationSpeed);
                break;
            case 2:
                rb.velocity = dashDirection * dashingSpeed;
                bossDetector.transform.rotation = Quaternion.LookRotation(dashDirection);
                break;
           
        }

        if (timeStamp <= 0 )
        {
            
            timeStamp = timeBetweenAttacks;

            if (!followCurve.moving)
            {
                if (!spawnedRabbit)
                {
                    state = 0;
                    spawnedRabbit = true;
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
            else
            {
                state = 0;
                StartCoroutine(DashRandom(0.2f));
            }
        }
        
        if (hurtAnim)
        {
            aggroMaterial.SetFloat("_UseOnAlbedo",animationHurt.Evaluate(Time.time - hurtTime));
            aggroMaterial.SetFloat("_UseOnEmission",animationHurt.Evaluate(Time.time - hurtTime));
            
            if (Time.time - hurtTime > animationHurt.keys[animationHurt.keys.Length - 1].time) hurtAnim = false;
        }

        if (shockWaveGameObject.activeSelf)
        {
            shockWaveGameObject.transform.localScale += new Vector3((Time.deltaTime * shockWaveSpeed),0,(Time.deltaTime * shockWaveSpeed));
        }

        if (legsColider.childCount > 3)
        {
            PlayerManager.instance.Rewind();
        }
    }

    public IEnumerator SpawnRabbits(float delay,float vfxDelay)
    {
        material.color = Color.cyan;
        BossRollerCoaster.returning = false;
        animator.Play("Lance-Lapin");
        yield return new WaitForSeconds(vfxDelay);
        vfx[1].SetActive(true);
        vfx[1].GetComponent<ParticleSystem>().Stop();
        vfx[1].GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(delay-vfxDelay);
        
        foreach (GameObject pos in spawnPosPillars)
        {
            pos.SetActive(true);
            GameObject vfxpillar = Instantiate(vfx[2], pos.transform.position, quaternion.identity);
            vfxpillar.transform.rotation = Quaternion.Euler(-90,0,0);
            Destroy(vfxpillar,3);
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
    
    public IEnumerator DashRandom(float delay)
    {
        animator.Play("Dash");
        material.color = Color.cyan;
        yield return new WaitForSeconds(delay);
        vfx[3].SetActive(true);
        vfx[3].GetComponent<ParticleSystem>().Stop();
        vfx[3].GetComponent<ParticleSystem>().Play();
        
        dashDirection = Quaternion.AngleAxis(Random.Range(0,360),Vector3.up) * Vector3.forward;
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
    
    public IEnumerator BreakRoller(float delay)
    {
        animator.Play("Attaque");
        yield return new WaitForSeconds(delay);
        material.color = Color.blue;
        GameObject attack = Instantiate(vfx[6], transform.position - new Vector3(0,0.35f,0) + bossDetector.transform.forward*2,quaternion.identity,attackSpawnPlace);
        attack.transform.SetParent(null);
        attack.transform.rotation = Quaternion.Euler(-90,0,0);
        BossRollerCoaster.returning = true;
        BossRollerCoaster.materialLight.SetFloat("_flick",0);
        yield return new WaitForSeconds(shockWaveDuration);
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
    public IEnumerator Mort(float delay, float delayMort)
    {
        animator.Play("StandUp");
        yield return new WaitForSeconds(delay);
        rb.isKinematic = true;
        animator.Play("Mort");
        GameObject death = Instantiate(vfx[5], DeathSpawnPlace);
        yield return new WaitForSeconds(delayMort);
        teleporter.StartTP();
        yield return new WaitForSeconds(1);
        Destroy(death);
        transform.position = afterBossPos.position;
        bossDetector.transform.rotation = quaternion.Euler(0,0,0);
        yield return new WaitForSeconds(1.2f);
        pnjDialoguesManager.StartDialogue();
        

    }
    public void GetHurt(int damage)
    {
        if (state == 3)
        {
            if (hurtVFX != null)
            {
                hurtVFX.Play();
            }
            hurtAnim = true;
            hurtTime = Time.time;
            if (phase < 2)
            {
                state = 0;
                StartCoroutine(StandUp(2));
                phase++;
                if (phase == 2)
                {
                    timeBetweenAttacks = timeBetweenAttackPhaseThree;
                    shockWaveDuration = shockWaveDuration2;
                    shockWaveSpeed = shockWaveSpeed3;
                    walkingSpeed = walkingSpeed3;
                }
            }
            else
            {
                state = 0;
                StartCoroutine(Mort(2, 3));
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
