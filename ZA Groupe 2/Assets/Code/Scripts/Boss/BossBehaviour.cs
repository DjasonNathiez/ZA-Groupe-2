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
    public bool fighting;
    
    
    
    public Rigidbody rb;
    [SerializeField] private GameObject shockWaveGameObject;
    
    [SerializeField] private float shockWaveSpeed = 1;
    [SerializeField] private float shockWaveDuration;
    [SerializeField] public List<GameObject> pillars;
    public Vector3[] pillarPos;
    
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
    public Transform afterBossPos;
    public PnjDialoguesManager firstDialogue;
    public PnjDialoguesManager lastDialogue;
    
    [Header("CAMERA")]
    
    public CameraController cameraController;
    public Vector3 arenaCenter;
    public Vector3 rot;
    public float zoom;
    public Vector3 pos;
    
    

    [Header("FIREWORKS")]
    public float timerFirework;
    public float delayFireWork;
    public GameObject firework;
    public float fireworkVelocity;
    public bool fireworks;
    public float fireworksTime;

    
    [Header("1ST TORNADO")]
    public AnimationCurve tornadoMovement;
    public int tornadoforce;
    public int tornadoAngle = 0;
    public float tornadoDuration;
    public float timeBuffer;
    public bool tornadoMove;
    public bool tornado;
    public bool doingTornado;
    public Vector3 tornadoDestination;
    public Vector3 tornadoFrom;
    public float tornadoTimer;
    public float[] tornadoDelay;
    public GameObject bullet;
    public float heightBullet = 2.6f;

    [Header("2ND TORNADO")] 
    public float speed;
    public Vector3 tornadoVelocity;
    


    [Header("JUMP")]
    public AnimationCurve jumpcurve;
    public bool jump;
    public bool jumped;
    public float yValue;
    public bool jumpFirst;
    public bool jumpLast;
    public float timeBufferJump;
    public Transform cible;
    public MeshRenderer cibleRender;
    public Material cibleMat;
    public Color cibleColor;

    [Header("VFX")]
    public ParticleSystem hurtVFX;

    public bool falling;
    public bool fallen;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        material = new Material(material);
        GetComponent<MeshRenderer>().material = material;
        StartCoroutine(StartFight());
        aggroMaterial = new Material(aggroMaterial);
        foreach (SkinnedMeshRenderer mesh in modelMeshRenderer)
        {
            mesh.material = aggroMaterial;
        }
        foreach (MeshRenderer mesh in modelMeshRenderer2)
        {
            mesh.material = aggroMaterial;
        }
        cibleRender = cible.GetComponent<MeshRenderer>();
        cibleMat = new Material(cibleRender.material);
        cibleRender.material = cibleMat;
        yValue = transform.position.y;
        cameraController = PlayerManager.instance.cameraController;
    }
    
    
    public IEnumerator StartFight()
    {
        yield return new WaitForSeconds(2);
        firstDialogue.StartDialogue();

    }

    void Update()
    {
        // GESTION DE LA CAMERA AUTOUR DE L'ARENE

        if (fighting)
        {
            cameraController.cameraPos.transform.rotation = Quaternion.Lerp(cameraController.cameraPos.transform.rotation,Quaternion.Euler(rot.x,Quaternion.LookRotation(-(PlayerManager.instance.transform.position - arenaCenter)).eulerAngles.y,rot.z),3*Time.deltaTime);
            cameraController.cameraPos.transform.position = arenaCenter + (PlayerManager.instance.transform.position - arenaCenter).normalized * 8;
            cameraController.cameraPos.transform.position = cameraController.cameraPos.transform.position + pos;
            cameraController.cameraZoom = zoom;   
        }
        else
        {
            if (firstDialogue.dialogueEnded)
            {
                fighting = true;
                cameraController.playerFocused = false;
                StartCoroutine(ShootFireworks(2.2f,1.2f));
            }
        }

        if (fireworks)
        {
            if (timerFirework > 0)
            {
                timerFirework -= Time.deltaTime;
            }
            else 
            {
                Vector3 pos = new Vector3(PlayerManager.instance.transform.position.x + Random.Range(-1.2f,1.2f),PlayerManager.instance.transform.position.y , PlayerManager.instance.transform.position.z + Random.Range(-1.2f,1.2f));
                Vector3 spawn = new Vector3(PlayerManager.instance.transform.position.x + Random.Range(-10f, 10f), 25, PlayerManager.instance.transform.position.z + Random.Range(-10f, 10f));
                
                GameObject newFirework = Instantiate(firework, spawn, Quaternion.LookRotation((pos - spawn)));
                newFirework.GetComponent<Rigidbody>().velocity = (pos - spawn).normalized * fireworkVelocity * Random.Range(0.7f,1.3f);
                newFirework.GetComponent<BossRocket>().destination = pos;
                newFirework.GetComponent<BossRocket>().velocity = fireworkVelocity;
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
                tornadoTimer = tornadoDelay[phase];
                switch (phase)
                {
                    case 0:
                        GameObject newBullet = Instantiate(bullet, transform.position + Vector3.down * heightBullet, Quaternion.Euler(90,0,0));
                        newBullet.GetComponent<bulletBehavior>().velocity = Quaternion.Euler(0,tornadoAngle,0) * Vector3.forward;
                        GameObject newBullettwo = Instantiate(bullet, transform.position + Vector3.down * heightBullet, Quaternion.Euler(90,0,0));
                        newBullettwo.GetComponent<bulletBehavior>().velocity = Quaternion.Euler(0,tornadoAngle,0) * Vector3.back;
                        tornadoAngle += tornadoforce;   
                        break;
                    case 1:
                        for (int i = tornadoAngle; i < tornadoAngle+360; i += 36)
                        {
                            GameObject newBulletthree = Instantiate(bullet, transform.position + Vector3.down * heightBullet, Quaternion.Euler(90,0,0));
                            newBulletthree.GetComponent<bulletBehavior>().velocity = Quaternion.Euler(0,i,0) * Vector3.forward;
                            tornadoAngle += 18;
                        }
                        break;
                    case 2:
                        GameObject newBulletfour = Instantiate(bullet, transform.position + Vector3.down * heightBullet, Quaternion.Euler(90,0,0));
                        newBulletfour.GetComponent<bulletBehavior>().velocity = PlayerManager.instance.transform.position + new Vector3(Random.Range(-2f,2f),0,Random.Range(-2f,2f)) - transform.position + Vector3.down * 2.4f;
                        GameObject newBulletfive = Instantiate(bullet, transform.position + Vector3.down * heightBullet, Quaternion.Euler(90,0,0));
                        newBulletfive.GetComponent<bulletBehavior>().velocity = Quaternion.Euler(0,120,0) * (PlayerManager.instance.transform.position + new Vector3(Random.Range(-2f,2f),0,Random.Range(-2f,2f)) - transform.position + Vector3.down * 2.4f);
                        GameObject newBulletsix = Instantiate(bullet, transform.position + Vector3.down * heightBullet, Quaternion.Euler(90,0,0));
                        newBulletsix.GetComponent<bulletBehavior>().velocity = Quaternion.Euler(0,240,0) * (PlayerManager.instance.transform.position + new Vector3(Random.Range(-2f,2f),0,Random.Range(-2f,2f)) - transform.position + Vector3.down * 2.4f);

                        break;
                }
            }

            if (phase == 1)
            {
                transform.Translate(tornadoVelocity.normalized * speed * Time.deltaTime);
            }
            else if (phase == 2)
            {
                transform.Translate(tornadoVelocity.normalized * speed * Time.deltaTime * 2);
            }
        }

        if (jump)
        {
            transform.position = new Vector3(transform.position.x, jumpcurve.Evaluate(Time.time - timeBuffer) + yValue,transform.position.z);

            if (Time.time - timeBuffer > jumpcurve.keys[1].time && !jumped)
            {
                jumped = true;
                transform.position = new Vector3(PlayerManager.instance.transform.position.x, jumpcurve.keys[1].value + yValue, PlayerManager.instance.transform.position.z);
                cibleMat.color = new Color(cibleColor.r, cibleColor.g, cibleColor.b, 0);
                cible.gameObject.SetActive(true);
                cible.position = new Vector3(PlayerManager.instance.transform.position.x, 6.8f, PlayerManager.instance.transform.position.z);
            }

            if (jumped)
            {
                cibleMat.color = Color.Lerp(cibleMat.color, cibleColor, Time.deltaTime * 2);
                cible.rotation = Quaternion.Euler(90,0,0);
            }
        }
        
        if (jumpFirst)
        {
            transform.position = new Vector3(transform.position.x, jumpcurve.Evaluate(Time.time - timeBufferJump) + yValue,transform.position.z);

            if (Time.time - timeBufferJump > jumpcurve.keys[1].time)
            {
                jumpFirst = false;
            }
        }
        
        if (jumpLast)
        {
            transform.position = new Vector3(transform.position.x, jumpcurve.Evaluate((Time.time - timeBufferJump) + jumpcurve.keys[1].time) + yValue,transform.position.z);

            if ((Time.time - timeBufferJump) + jumpcurve.keys[1].time > jumpcurve.keys[2].time)
            {
                jumpLast = false;
            }
            cibleMat.color = Color.Lerp(cibleMat.color, cibleColor, Time.deltaTime * 2);
            cible.rotation = Quaternion.Euler(90,0,0);
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

    public IEnumerator ShootFireworks(float delay,float vfxDelay)
    {
        animator.Play("FireworkLaunch");
        yield return new WaitForSeconds(vfxDelay);
        vfx[1].SetActive(true);
        vfx[1].GetComponent<ParticleSystem>().Stop();
        vfx[1].GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(delay-vfxDelay);
        
        state = 1;
        fireworks = true;
        
        yield return new WaitForSeconds(1);
        animator.Play("Attaque");
        jumpFirst = true;
        timeBufferJump = Time.time;

        for (int i = 0; i < 4; i++)
        {
            if (pillars[i].activeSelf == false)
            {
                pillars[i].transform.localPosition = pillarPos[i];
                pillars[i].SetActive(true);
            }
        }

        yield return new WaitForSeconds(fireworksTime-1);
        
        fireworks = false;
        
        yield return new WaitForSeconds(3);
        
        animator.Play("Attaque");
        jumpLast = true;
        cibleMat.color = new Color(cibleColor.r, cibleColor.g, cibleColor.b, 0);
        cible.gameObject.SetActive(true);
        cible.position = new Vector3(PlayerManager.instance.transform.position.x, 6.8f, PlayerManager.instance.transform.position.z);
        timeBufferJump = Time.time;
        transform.position = new Vector3(PlayerManager.instance.transform.position.x, jumpcurve.keys[1].value + yValue, PlayerManager.instance.transform.position.z);
        
        yield return new WaitForSeconds(jumpcurve.keys[2].time - jumpcurve.keys[1].time);
        cible.gameObject.SetActive(false);
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
        bool refill = true;
        foreach (GameObject obj in pillars)
        {
            if (obj.activeSelf)
            {
                refill = false;
                break;
            }
        }

        if (refill)
        {
            StartCoroutine(ShootFireworks(2.2f,1.2f));
        }
        else
        {
            StartCoroutine(Tornado());   
        }
    }

    public IEnumerator Tornado()
    {
        animator.Play("TornadoStart");
        material.color = Color.cyan;
        doingTornado = true;
        yield return new WaitForSeconds(1);
        if (phase == 0)
        {
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
        }
        else
        {
            if (doingTornado)
            {
                animator.Play("TornadoMiddle");
                tornado = true;
                tornadoVelocity = PlayerManager.instance.transform.position - transform.position;
                tornadoVelocity = new Vector3(tornadoVelocity.x, 0, tornadoVelocity.z);
            }
            yield return new WaitForSeconds(tornadoDuration);
            if (doingTornado)
            {
                tornadoDestination = new Vector3(arenaCenter.x,transform.position.y,arenaCenter.z) + (transform.position - new Vector3(arenaCenter.x,transform.position.y,arenaCenter.z).normalized * 8);
                tornadoDestination = new Vector3(tornadoDestination.x, transform.position.y, tornadoDestination.z);
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
        }
        
        yield return new WaitForSeconds(2);
        if (doingTornado)
        {
            doingTornado = false;
            StartCoroutine(Jump());
        }
        

    }

    private void OnCollisionEnter(Collision other)
    {
        if (phase > 0 && tornado && other.transform.CompareTag("BossWall"))
        {
            tornadoVelocity = Vector3.Reflect(tornadoVelocity, other.contacts[0].normal);
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
        cible.gameObject.SetActive(false);
        shockWaveGameObject.transform.position = transform.position - new Vector3(0,2.64f,0);
        shockWaveGameObject.transform.localScale = new Vector3(2, 1, 2);
        GameObject attack = Instantiate(vfx[6], transform.position - new Vector3(0,0.35f,0),quaternion.identity,attackSpawnPlace);
        attack.transform.SetParent(null);
        attack.transform.rotation = Quaternion.Euler(-90,0,0);
        yield return new WaitForSeconds(shockWaveDuration);
        shockWaveGameObject.SetActive(false);
        material.color = Color.blue;
        bool refill = true;
        foreach (GameObject obj in pillars)
        {
            if (obj.activeSelf)
            {
                refill = false;
                break;
            }
        }

        if (refill)
        {
            StartCoroutine(ShootFireworks(2.2f,1.2f));
        }
        else
        {
            StartCoroutine(Tornado());   
        }

    }

    public IEnumerator Fall(Vector3 dir)
    {
        hurtAnim = true;
        hurtTime = Time.time;
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
        falling = false;

    }
    
    public IEnumerator Mort(float delay, float delayMort)
    {
        animator.Play("StandUp");
        yield return new WaitForSeconds(delay);
        rb.isKinematic = true;
        animator.Play("Mort");
        GameObject death = Instantiate(vfx[5], DeathSpawnPlace);
        yield return new WaitForSeconds(delayMort);
        yield return new WaitForSeconds(1);
        Destroy(death);
        transform.position = afterBossPos.position;
        yield return new WaitForSeconds(1.2f);


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
        StartCoroutine(ShootFireworks(2.2f,1.2f));
    }
}

[Serializable]
public class Pilone
{
    public GameObject pilone;
    public float rotation;
}
