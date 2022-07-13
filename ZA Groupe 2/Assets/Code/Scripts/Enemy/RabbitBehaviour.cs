using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RabbitBehaviour : AIBrain
{
    #region Variables

    [Header("Rabbit Self Data")] public float areaToMove;
    private Vector3 m_pointToGo;
    public float avoidFront;
    private Vector3 m_originPoint;

    public StateMachine stateMachine;

    public enum StateMachine
    {
        IDLE,
        CHASE,
        ATTACK
    }

    public List<Vector3> detectedPoints;
    public List<float> distanceToPoints;
    public Vector3 nearestPoint;

    [Header("Rabbit Self VFX")] public ParticleSystem chaseBegin;
    public ParticleSystem chaseContinue;
    public ParticleSystem chaseEnd;
    public ParticleSystem chaseSurprise;
    public ParticleSystem chaseMangaAngry;

    #endregion

    void Start()
    {
        isInvincible = false;
        m_originPoint = transform.position;

        InitializationData();

        SwitchState(StateMachine.IDLE);
    }

    public override void Update()
    {
        base.Update();

        if (!isDead)
        {
            CheckState();
            Detection();
        }
    }

    public override void Detection()
    {
        base.Detection();
    }

    public override void SetAnimator()
    {
        base.SetAnimator();

        if (hurtAnim)
        {
            modelAggroMat.SetFloat("_UseOnAlbedo", animationHurt.Evaluate(Time.time - hurtTime));
            modelAggroMat.SetFloat("_UseOnEmission", animationHurt.Evaluate(Time.time - hurtTime));
            modelNonAggroMat.SetFloat("_UseOnAlbedo", animationHurt.Evaluate(Time.time - hurtTime));
            modelNonAggroMat.SetFloat("_UseOnEmission", animationHurt.Evaluate(Time.time - hurtTime));
            if (Time.time - hurtTime > animationHurt.keys[animationHurt.keys.Length - 1].time) hurtAnim = false;
        }

        if (isDead)
        {
            modelAggroMat.SetFloat("_NoiseStrenght", animationDeath.Evaluate(Time.time - hurtTime));
            modelNonAggroMat.SetFloat("_NoiseStrenght", animationDeath.Evaluate(Time.time - hurtTime));
        }
    }

    private void CheckState()
    {
        switch (stateMachine)
        {
            case StateMachine.IDLE:

                // Moving Idle
                Vector3 pointToGoMin = new Vector3(m_pointToGo.x - 3, transform.position.y, m_pointToGo.z - 3);
                Vector3 pointToGoMax = new Vector3(m_pointToGo.x + 3, transform.position.y, m_pointToGo.z + 3);

                if (transform.position.x >= pointToGoMin.x && transform.position.z >= pointToGoMin.z &&
                    transform.position.x <= pointToGoMax.x && transform.position.z <= pointToGoMax.z)
                {
                    SetNavPoint();
                    nav.SetDestination(m_pointToGo);
                }

                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
                {
                    float distToFrontHit = Vector3.Distance(transform.forward, hit.point);

                    if (distToFrontHit <= avoidFront)
                    {
                        SetNavPoint();
                        nav.SetDestination(m_pointToGo);
                    }
                }

                if (nav.path.status != NavMeshPathStatus.PathComplete)
                {
                    m_pointToGo = m_originPoint;
                    nav.SetDestination(m_pointToGo);
                }

                // Aggro Idle
                if (player.state == ActionType.RopeAttached && !player.rope.rewinding)
                {
                    if (distanceToPlayer <= dectectionRange)
                    {
                        if (!isAggro) SwitchState(StateMachine.CHASE);
                    }
                }

                break;

            case StateMachine.CHASE:

                RopePointDetection();
                MoveToRope();

                float distanceToNearestPoint = Vector3.Distance(transform.position, nearestPoint);
                if (attackRange > distanceToNearestPoint) SwitchState(StateMachine.ATTACK);

                break;

            case StateMachine.ATTACK:
                isAttacking = true;
                player.rope.rewinding = true;
                SwitchState(StateMachine.IDLE);
                break;
        }
    }

    private void SwitchState(StateMachine state)
    {
        switch (state)
        {
            case StateMachine.IDLE:
                chaseContinue.Stop();
                chaseEnd.Play();

                isAggro = false;
                isAttacking = false;
                isMoving = false;

                distanceToPoints.Clear();
                detectedPoints.Clear();
                nearestPoint = Vector3.zero;

                m_pointToGo = m_originPoint;
                nav.SetDestination(m_pointToGo);

                stateMachine = StateMachine.IDLE;
                break;

            case StateMachine.CHASE:
                chaseBegin.Play();
                chaseContinue.Play();
                chaseSurprise.Play();
                chaseMangaAngry.Play();
                PlaySFX("R_Aggro");

                isAggro = true;
                isMoving = true;
                isAttacking = false;

                distanceToPoints.Clear();
                detectedPoints.Clear();
                nearestPoint = Vector3.zero;

                stateMachine = StateMachine.CHASE;
                break;

            case StateMachine.ATTACK:
                animator.Play("R_Attack");

                stateMachine = StateMachine.ATTACK;
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
        if (player.state != ActionType.RopeAttached)
        {
            SwitchState(StateMachine.IDLE);
            return;
        }

        if (player.rope.enabled)
        {
            if (player.rope.rewinding)
            {
                SwitchState(StateMachine.IDLE);
                return;
            }

            detectedPoints = player.rope.CalculateCuttingPoints(1);

            if (detectedPoints.Count > distanceToPoints.Count) AddPointDistanceToRope();

            if (distanceToPoints.Count > 0) CheckNearestPoint();
            else nearestPoint = player.transform.position;
        }
        else SwitchState(StateMachine.IDLE);
    }

    public override void GetHurt(int damage)
    {
        base.GetHurt(damage);
    }

    private void AddPointDistanceToRope()
    {
        for (int i = distanceToPoints.Count; i < detectedPoints.Count; i++)
        {
            distanceToPoints.Add(Vector3.Distance(transform.position, detectedPoints[i]));
        }
    }

    private void CheckNearestPoint()
    {
        var minDistance = Mathf.Infinity;
        var nearest = Vector3.zero;
        for (int i = 0; i < distanceToPoints.Count; i++)
        {
            if (distanceToPoints[i] < minDistance)
            {
                if (i > distanceToPoints.Count - 1 || i > detectedPoints.Count - 1)
                {
                    Debug.LogWarning("Index invalide");
                    nearest = player.transform.position;
                    continue;
                }
                minDistance = distanceToPoints[i];
                nearest = detectedPoints[i];
            }
        }

        nearestPoint = nearest;
    }

    public void PlaySFX(string soundName)
    {
        foreach (AudioManager.Sounds s in AudioManager.instance.rabbitSounds)
        {
            if (s.soundName == soundName)
            {
                if (s.loop)
                {
                    SoundManager.PlayFx(s.clip, loop: true, volume: s.volume);
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