using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitBehaviour : AIBrain
{
    [Header("Rabbit Self Data")]
    
    public float areaToMove;
    private Vector3 m_pointToGo;
    public float avoidFront;
    private Vector3 m_originPoint;
    public float timeToGoNextPoint;
    
    [HideInInspector] public StateMachine stateMachine;
    public enum StateMachine{IDLE, CHASE, ATTACK}

    public bool isPatrolling;

    [HideInInspector] public Vector3[] detectedPoints;
    [HideInInspector] public List<float> distanceToPoints;
    [HideInInspector] public Vector3 nearestPoint;

    private Material enemyStatutPointerMaterial;

    [Header("Rabbit Self VFX")] 
    public ParticleSystem chaseBegin;
    public ParticleSystem chaseContinue;
    public ParticleSystem chaseEnd;
    public ParticleSystem chaseSurprise;
    public ParticleSystem chaseMangaAngry;
    
    void Start()
    {
        isInvincible = false;
        m_originPoint = transform.position;
        enemyStatutPointerMaterial = enemyStatusPointer.GetComponent<MeshRenderer>().material;
        
        float minX = transform.position.x - areaToMove;
        float minZ = transform.position.z - areaToMove;
        float maxX = transform.position.x + areaToMove;
        float maxZ = transform.position.z + areaToMove;

        m_pointToGo = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minZ, maxZ));
        
        InitializationData();
    }

    void Update()
    {
        //Animator Set Bool
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isPatrolling", isPatrolling);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isHurt", isHurt);
        animator.SetBool("isDead", isDead);
        
        if (hurtAnim)
        {
            modelAggroMat.SetFloat("_UseOnAlbedo",animationHurt.Evaluate(Time.time - hurtTime));
            modelAggroMat.SetFloat("_UseOnEmission",animationHurt.Evaluate(Time.time - hurtTime));
            modelNonAggroMat.SetFloat("_UseOnAlbedo",animationHurt.Evaluate(Time.time - hurtTime));
            modelNonAggroMat.SetFloat("_UseOnEmission",animationHurt.Evaluate(Time.time - hurtTime));
            if (Time.time - hurtTime > animationHurt.keys[animationHurt.keys.Length - 1].time) hurtAnim = false;
        }
        if (isDead)
        {
            modelAggroMat.SetFloat("_NoiseStrenght",animationDeath.Evaluate(Time.time - hurtTime));
            modelNonAggroMat.SetFloat("_NoiseStrenght",animationDeath.Evaluate(Time.time - hurtTime));
        }
        if (!isDead)
        {
            CheckState();
            Detection();
            SetColor();
        }

        enemyStatutPointerMaterial = isAggro ? aggroMaterial : nonAggroMaterial;
        enemyStatusPointer.SetActive(isAggro);
    }

    private void CheckState()
    {
        float distanceToNearestPoint = Vector3.Distance(transform.position, nearestPoint);
        
        if (player.GetComponent<PlayerManager>().state != "StatusQuo")
        {
            if (distanceToPlayer <= dectectionRange && dectectionRange < distanceToNearestPoint && !isAggro)
            {
                chaseBegin.Play();
                chaseContinue.Play();
                chaseSurprise.Play();
                chaseMangaAngry.Play();
                PlaySFX("R_Aggro");
                
                isAggro = true;
                stateMachine = StateMachine.CHASE;
            }

            stateMachine = attackRange > distanceToNearestPoint ? StateMachine.ATTACK : StateMachine.CHASE;
            
        }
        else
        {
            if (isAggro)
            {
                stateMachine = StateMachine.IDLE;
            
                chaseContinue.Stop();
                chaseEnd.Play();
            
                isAggro = false;
            }
        }

        if (isAggro)
        {
            RopePointDetection();
        }

        switch (stateMachine)
        {
            case StateMachine.IDLE:
                if (!isPatrolling)
                {
                    isAttacking = false;
                    isPatrolling = true;
                    isMoving = false;
                    isAggro = false;
                }

                Vector3 pointToGoMin = new Vector3(m_pointToGo.x - 3, transform.position.y, m_pointToGo.z - 3);
                Vector3 pointToGoMax = new Vector3(m_pointToGo.x + 3, transform.position.y, m_pointToGo.z + 3);

                if (transform.position.x >= pointToGoMin.x && transform.position.z >= pointToGoMin.z &&
                    transform.position.x <= pointToGoMax.x && transform.position.z <= pointToGoMax.z)
                {
                    SetNavPoint();
                }

                RaycastHit hit; 
                if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
                {
                    float distToFrontHit = Vector3.Distance(transform.forward, hit.point);
                            
                    if (distToFrontHit <= avoidFront)
                    {
                        SetNavPoint();
                    }
                }
                
                nav.SetDestination(m_pointToGo);
                
                break;

            case StateMachine.CHASE:
                isAggro = true;
                isMoving = true;
                isPatrolling = false;
                isAttacking = false;
                
                MoveToRope();
                
                break;
            
            case StateMachine.ATTACK:
                isAttacking = true;
                player.GetComponent<PlayerManager>().rope.rewinding = true;
                break;
        }
    }

    private void SetNavPoint()
    {
        float minX = m_originPoint.x - areaToMove;
        float minZ = m_originPoint.z - areaToMove;
        float maxX = m_originPoint.x + areaToMove;
        float maxZ = m_originPoint.z + areaToMove;

        m_pointToGo = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minZ, maxZ));
    }

    private void MoveToRope()
    {
        if (isAggro)
        {
            isMoving = true;
            nav.SetDestination(nearestPoint);
        }
    }

    private void RopePointDetection()
    {
        if (player.GetComponent<PlayerManager>().rope.enabled)
        {
            detectedPoints = player.GetComponent<Rope>().CalculateCuttingPoints(1);

            if (detectedPoints.Length > distanceToPoints.Count)
            {
                AddPointDistanceToRope();
            }
            
            
            if (distanceToPoints.Count > 0)
            {
                CheckNearestPoint();
            }
        }
        else
        {
            nearestPoint = Vector3.zero;
        }

        if (distanceToPoints != null && detectedPoints != null)
        {
            if (distanceToPoints.Count > detectedPoints.Length)
            {
                List<int> indexes = new List<int>(0);

                for (int x = 0; x < distanceToPoints.Count; x++)
                {
                    for (int i = detectedPoints.Length; i < distanceToPoints.Count; i++)
                    {
                        indexes.Add(x);
                    }
                }

                for (int i = indexes.Count-1; i > -1; i--)
                {
                    if(i > distanceToPoints.Count -1) return;
                    distanceToPoints.RemoveAt(i);
                }
            }
        }
        

    }

    private void AddPointDistanceToRope()
    {
        for (int i = distanceToPoints.Count; i < detectedPoints.Length; i++)
        {
            distanceToPoints.Add(Vector3.Distance(transform.position, detectedPoints[i]));
        }
    }

    private void CheckNearestPoint()
    {
        for (int i = 0; i < distanceToPoints.Count - 1 ; i++)
        {
            if (distanceToPoints[i] < distanceToPoints[i + 1])
            {
                if(i> detectedPoints.Length -1)
                    return;
                nearestPoint = detectedPoints[i];
            }
        }
    }
    
    public void PlaySFX(string soundName)
    {
        foreach (AudioManager.Sounds s in AudioManager.instance.rabbitSounds)
        {
            if (s.soundName == soundName)
            {
                if (s.loop)
                {
                    SoundManager.PlayFx(s.clip,loop: true, volume: s.volume);
                }
                else
                {
                    SoundManager.PlayOnce(s.clip, volume: s.volume, mixer: SoundManager.FxAudioMixer);
                }
            }
        }
    }

    public void StopSFX(string soundName)
    {
        foreach (AudioManager.Sounds s in AudioManager.instance.rabbitSounds)
        {
            if (s.soundName == soundName)
            {
                SoundManager.StopFx(s.clip);
            }
        }
    }
}
