using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BossBehaviour : MonoBehaviour
{
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
    public float tornadoAngle = 0;
    public float tornadoDuration;
    public float timeBuffer;
    public bool tornadoMove;
    public bool tornado;
    public bool doingTornado;
    public Vector3 tornadoDestination;
    public Vector3 tornadoFrom;
    public float tornadoTimer;
    public float tornadoDelay;
    public GameObject bullet;


    [Header("JUMP")]
    public AnimationCurve jumpcurve;
    public bool jump;
    public bool jumped;
    public float yValue;

    [Header("VFX")]
    public ParticleSystem hurtVFX;

    public bool falling;
    public bool fallen;


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

        yValue = transform.position.y;
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
        
        if (falling)
        {
            transform.position = Vector3.Lerp(transform.position, tornadoDestination,
                Time.deltaTime*5);
        }

        if (tornado)
        {
            if (tornadoTimer > 0)
            {
                tornadoTimer -= Time.deltaTime;
            }
            else 
            {
                GameObject newBullet = Instantiate(bullet, transform.position + Vector3.down * 2.4f, Quaternion.Euler(90,0,0));
                newBullet.GetComponent<bulletBehavior>().velocity = Quaternion.Euler(0,tornadoAngle,0) * Vector3.forward;
                tornadoTimer = tornadoDelay + Random.Range(-tornadoDelay/3,tornadoDelay/3);
                tornadoAngle += tornadoforce;
            }
        }

        if (jump)
        {
            transform.position = new Vector3(transform.position.x, jumpcurve.Evaluate(Time.time - timeBuffer) + yValue,transform.position.z);

            if (Time.time - timeBuffer > jumpcurve.keys[1].time && !jumped)
            {
                jumped = true;
                transform.position = new Vector3(PlayerManager.instance.transform.position.x, jumpcurve.keys[1].value + yValue, PlayerManager.instance.transform.position.z);
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
        StartCoroutine(Jump());
    }

    /*public IEnumerator Tornado()
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
        tornadoDestination = new Vector3(arenaCenter.x,transform.position.y,arenaCenter.z) - (tornadoFrom - new Vector3(arenaCenter.x,transform.position.y,arenaCenter.z));
        tornadoFrom = transform.position;
        timeBuffer = Time.time;
        tornadoMove = true;
        tornado = false;
        yield return new WaitForSeconds(tornadoMovement.keys[tornadoMovement.keys.Length - 1].time);
        animator.Play("TornadoEnd");
        tornadoMove = false;
        yield return new WaitForSeconds(2);
        StartCoroutine(Jump());
        

    }*/
    
    public IEnumerator Tornado()
    {
        animator.Play("TornadoStart");
        material.color = Color.cyan;
        doingTornado = true;
        yield return new WaitForSeconds(1);
        if (doingTornado)
        {
            animator.Play("TornadoMiddle");
            tornadoDestination = new Vector3(arenaCenter.x,transform.position.y,arenaCenter.z);
            tornadoFrom = transform.position;
            timeBuffer = Time.time;
            tornadoMove = true;   
        }
        yield return new WaitForSeconds(tornadoMovement.keys[tornadoMovement.keys.Length - 1].time);
        if (doingTornado)
        {
            tornado = true;
            tornadoAngle = Random.Range(0, 360);
            transform.position = tornadoDestination;
            tornadoMove = false;
            timeBuffer = Time.time;
        }
        yield return new WaitForSeconds(tornadoDuration);
        if (doingTornado)
        {
            tornadoDestination = new Vector3(arenaCenter.x,transform.position.y,arenaCenter.z) - (tornadoFrom - new Vector3(arenaCenter.x,transform.position.y,arenaCenter.z));
            tornadoFrom = transform.position;
            timeBuffer = Time.time;
            tornadoMove = true;
            tornado = false;
        }
        yield return new WaitForSeconds(tornadoMovement.keys[tornadoMovement.keys.Length - 1].time);
        if (doingTornado)
        {
            animator.Play("TornadoEnd");
            tornadoMove = false;
        }
        yield return new WaitForSeconds(2);
        if (doingTornado)
        {
            doingTornado = false;
            StartCoroutine(Jump());
        }
        

    }
    
    public IEnumerator Jump()
    {
        animator.Play("Attaque");
        material.color = Color.cyan;
        jump = true;
        timeBuffer = Time.time;
        yield return new WaitForSeconds(jumpcurve.keys[jumpcurve.keys.Length - 1].time);
        jump = false;
        jumped = false;
        transform.position = new Vector3(transform.position.x,yValue,transform.position.z);
        shockWaveGameObject.SetActive(true);
        shockWaveGameObject.transform.position = transform.position - new Vector3(0,2.64f,0);
        shockWaveGameObject.transform.localScale = new Vector3(2, 1, 2);
        GameObject attack = Instantiate(vfx[6], transform.position - new Vector3(0,0.35f,0),quaternion.identity,attackSpawnPlace);
        attack.transform.SetParent(null);
        attack.transform.rotation = Quaternion.Euler(-90,0,0);
        yield return new WaitForSeconds(shockWaveDuration);
        shockWaveGameObject.SetActive(false);
        material.color = Color.blue;
        StartCoroutine(Tornado());
        
    }

    public IEnumerator Fall(Vector3 dir)
    {
        tornado = false;
        falling = true;
        tornadoMove = false;
        doingTornado = false;
        tornadoDestination = new Vector3(arenaCenter.x, transform.position.y, arenaCenter.z) + dir.normalized * 8.2f;
        Debug.Log("Chute");
        animator.Play("Chute");
        rb.isKinematic = true;
        yield return new WaitForSeconds(2);
        fallen = true;

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
        if (fallen)
        {
            fallen = false;
            if (hurtVFX != null)
            {
                hurtVFX.Play();
            }
            hurtAnim = true;
            hurtTime = Time.time;
            if (phase < 2)
            {
                StartCoroutine(StandUp(2));
                phase++;
            }
            else
            {
                StartCoroutine(Mort(2, 3));
            }
        }
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
        StartCoroutine(ShootFireworks(2.2f,1.2f,5));
    }
}

[Serializable]
public class Pilone
{
    public GameObject pilone;
    public float rotation;
}
