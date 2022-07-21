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
    [SerializeField] private int phase;
    
    
    [SerializeField] private float dashingSpeed;
    [FormerlySerializedAs("m_rb")] [SerializeField]
    public Rigidbody rb;
    [SerializeField] private GameObject shockWaveGameObject;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float timeStamp;
    [SerializeField] private float shockWaveSpeed = 1;
    [SerializeField] private float shockWaveDuration;
    [SerializeField] private Vector3 dashDirection;
    [SerializeField] public List<GameObject> pillars;
    [SerializeField] private bossDetector bossDetector;
    [SerializeField] private Material material;
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
    
    public CameraController cameraController;
    public Vector3 arenaCenter;

    [Header("FIREWORKS")]
    public float timerFirework;
    public float delayFireWork;
    public GameObject firework;
    public Vector3 fireworkVelocity;
    public bool fireworks;

    
    [Header("TORNADO")]
    public AnimationCurve tornadoMovement;
    public AnimationCurve tornadoforceOverTime;
    public float tornadoforce;
    public float tornadoDuration;
    public float timeBuffer;
    public bool tornadoMove;
    public bool tornado;
    public Vector3 tornadoDestination;
    public Vector3 tornadoFrom;

    
    [Header("VFX")]
    public ParticleSystem hurtVFX;


    private void Start()
    {
        rb = GetComponent<Rigidbody>(); timeStamp = timeBetweenAttacks;
        material = new Material(material);
        GetComponent<MeshRenderer>().material = material;
        StartCoroutine(ShootFireworks(2.2f,1.2f,5));
        aggroMaterial = new Material(aggroMaterial);
        foreach (SkinnedMeshRenderer mesh in modelMeshRenderer)
        {
            mesh.material = aggroMaterial;
        }
        foreach (MeshRenderer mesh in modelMeshRenderer2)
        {
            mesh.material = aggroMaterial;
        }

        cameraController = PlayerManager.instance.cameraController;
        cameraController.playerFocused = false;
    }

    void Update()
    {
        // GESTION DE LA CAMERA AUTOUR DE L'ARENE

        cameraController.cameraPos.transform.rotation = Quaternion.Euler(25,Quaternion.LookRotation(-(PlayerManager.instance.transform.position - arenaCenter)).eulerAngles.y,0);
        cameraController.cameraPos.transform.position = arenaCenter + (PlayerManager.instance.transform.position - arenaCenter).normalized * 8;
        cameraController.cameraPos.transform.position = new Vector3(cameraController.cameraPos.transform.position.x, 10.7f, cameraController.cameraPos.transform.position.z);



        if (fireworks)
        {
            if (timerFirework > 0)
            {
                timerFirework -= Time.deltaTime;
            }
            else 
            {
                Vector3 pos = new Vector3(PlayerManager.instance.transform.position.x + Random.Range(-0.8f,0.8f), 25, PlayerManager.instance.transform.position.z + Random.Range(-0.5f,0.5f));
                GameObject newFirework = Instantiate(firework, pos, Quaternion.Euler(90,0,0));
                newFirework.GetComponent<Rigidbody>().velocity = fireworkVelocity * Random.Range(0.7f,1.3f);
                timerFirework = delayFireWork + Random.Range(-delayFireWork/3,delayFireWork/3);
            }    
        }

        if (tornadoMove)
        {
            transform.position = Vector3.Lerp(tornadoFrom, tornadoDestination,
                tornadoMovement.Evaluate(Time.time - timeBuffer));
        }

        if (tornado)
        {
            Vector3 force = (PlayerManager.instance.transform.position - new Vector3(arenaCenter.x, PlayerManager.instance.transform.position.y, arenaCenter.z)).normalized * tornadoforceOverTime.Evaluate((Time.time - timeBuffer)/tornadoDuration);
            PlayerManager.instance.rb.AddForce(force * tornadoforce,ForceMode.Force);
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
        
    }

    public IEnumerator ShootFireworks(float delay,float vfxDelay,float afterDelay)
    {
        animator.Play("FireworkLaunch");
        yield return new WaitForSeconds(vfxDelay);
        vfx[1].SetActive(true);
        vfx[1].GetComponent<ParticleSystem>().Stop();
        vfx[1].GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(delay-vfxDelay);
        
        material.color = Color.white;
        state = 1;
        fireworks = true;
        
        yield return new WaitForSeconds(afterDelay);
        
        fireworks = false;
        StartCoroutine(Tornado());
    }

    public IEnumerator Tornado()
    {
        animator.Play("TornadoStart");
        material.color = Color.cyan;
        yield return new WaitForSeconds(1);
        animator.Play("TornadoMiddle");
        tornadoDestination = new Vector3(arenaCenter.x,transform.position.y,arenaCenter.z);
        tornadoFrom = transform.position;
        timeBuffer = Time.time;
        tornadoMove = true;
        yield return new WaitForSeconds(tornadoMovement.keys[tornadoMovement.keys.Length - 1].time);
        tornado = true;
        transform.position = tornadoDestination;
        tornadoMove = false;
        timeBuffer = Time.time;
        yield return new WaitForSeconds(tornadoDuration);
        tornadoDestination = new Vector3(arenaCenter.x,transform.position.y,arenaCenter.z) + (tornadoFrom - new Vector3(arenaCenter.x,transform.position.y,arenaCenter.z));
        tornadoFrom = transform.position;
        timeBuffer = Time.time;
        tornadoMove = true;
        tornado = false;
        yield return new WaitForSeconds(tornadoMovement.keys[tornadoMovement.keys.Length - 1].time);
        animator.Play("TornadoEnd");
        tornadoMove = false;
        yield return new WaitForSeconds(2);
        StartCoroutine(ShootFireworks(2.2f,1.2f,5));
        

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
        if (hurtVFX != null)
        {
            hurtVFX.Play();
        }
        hurtAnim = true;
        hurtTime = Time.time;
           
        
    }
}

[Serializable]
public class Pilone
{
    public GameObject pilone;
    public float rotation;
}
