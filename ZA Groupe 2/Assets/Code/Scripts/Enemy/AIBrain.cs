using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AIBrain : MonoBehaviour
{
    [HideInInspector] public SpawnArea spawnPoint;

    [HideInInspector] public Rigidbody rb;
    public PlayerManager player;
    public NavMeshAgent nav;
    [HideInInspector] public Animator animator;
    [HideInInspector] public float distanceToPlayer;

    [HideInInspector] public bool isEnable = true;

    [Header("State")] public int currentHealth;
    public int maxHealth;
    [HideInInspector] public bool isInvincible;

    public float fallTime;
    [HideInInspector] public float timeOnGround;

    public bool canFall;
    public bool canMove = true;
    public bool canAttack = true;
    public bool canBeKnocked;

    [HideInInspector] public bool isFalling;
    public bool isAttacking;
    public bool isAggro;
    [HideInInspector] public bool isKnocked;
    public bool isMoving;
    public bool isDead;
    public bool isHurt;

    [Header("Movement")] public float moveSpeed;

    [Header("Attack")] public int attackDamage;
    public float attackRange;
    public float attackSpeed;
    public float attackDelay;
    [HideInInspector] public float activeAttackCd;
    [HideInInspector] public bool attackOnCd;
    public float knockbackForce;

    [Header("Detection")] public float dectectionRange;
    public float massAggroRange;
    [Range(0, 180)] public float detectionAngle;
    public Door doorIfDead;
    public ArenaParc currentArena;

    [Header("VFX")] public ParticleSystem hurtVFX;
    public ParticleSystem attackVFX;
    public ParticleSystem hitZoneVFX;
    public ParticleSystem deathVFX;
    public ParticleSystem explosionVFX;
    public bool explodeOnEvent;
    public ParticleSystem aggroVFX;
    public ParticleSystem counterVFX;

    [Header("Visual")] public List<SkinnedMeshRenderer> modelMeshRenderer;
    public Material modelNonAggroMat;
    public Material modelAggroMat;
    public Material aggroMaterial;
    public Material nonAggroMaterial;
    public AnimationCurve animationHurt;
    public AnimationCurve animationDeath;
    public bool hurtAnim;
    public float hurtTime;

    [Header("Drop")] [SerializeField] private int dropRate;

    public void ExplosionVFX()
    {
        if (explosionVFX != null)
        {
            explosionVFX.Play();
        }
    }

    public void InitializationData()
    {
        currentHealth = maxHealth;
        canMove = true;
        Enable();
        modelAggroMat = new Material(modelAggroMat);
        modelNonAggroMat = new Material(modelNonAggroMat);
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        player = PlayerManager.instance;
        nav = GetComponent<NavMeshAgent>();

        nav.speed = moveSpeed;
        nav.stoppingDistance = attackRange + 0.02f;
    }

    public virtual void Update()
    {
        SetColor();
        SetAnimator();
    }

    public virtual void SetAnimator()
    {
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isDead", isDead);
        animator.SetBool("isHurt", isHurt);
        animator.SetBool("isMoving", isMoving);
    }

    public virtual void MoveToPlayer(Vector3 destination)
    {
        isMoving = true;
        nav.SetDestination(destination);
    }

    public void SetColor()
    {
        foreach (SkinnedMeshRenderer mesh in modelMeshRenderer)
        {
            if (isAggro)
            {
                mesh.material = modelAggroMat;
            }
            else
            {
                mesh.material = modelNonAggroMat;
            }
        }
    }

    public virtual void Detection()
    {
        //Detect the player to aggro
        if (player == null) return;
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        //Mass Aggro AI Comportement
        if (isAggro)
        {
            Collider[] hitMass = Physics.OverlapSphere(transform.position, massAggroRange);

            foreach (Collider c in hitMass)
            {
                var neighborAI = c.GetComponent<AIBrain>();
                if (neighborAI == null) continue;
                if (neighborAI.GetComponent<RabbitBehaviour>()) continue;

                if (neighborAI && !neighborAI.isAggro)
                {
                    neighborAI.isAggro = true;
                }
            }
        }

        
    }

    public void AttackPlayer()
    {
        isAttacking = true;
        isMoving = false;
        canAttack = false;
    }

    public IEnumerator WaitForCooldown()
    {
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }

    public void AttackVFX()
    {
        if (attackVFX != null) attackVFX.Play();
    }

    public void AttackExit()
    {
        isAttacking = false;
        canMove = true;
    }

    public void FallOnTheGround()
    {
        LionBehaviour lionBehaviour = GetComponent<LionBehaviour>();
        
        rb.velocity = Vector3.zero;

        //Set State
        isFalling = true;
        isInvincible = false;
        isAttacking = false;
        canAttack = false;
        canMove = false;

        if (lionBehaviour)
        {
            lionBehaviour.fallVFX.Play();
        }

        if (hitZoneVFX)
        {
            hitZoneVFX.gameObject.SetActive(true);
        }

        StartCoroutine(WaitForStand());
    }

    #region Routine

    public IEnumerator WaitForStand()
    {
        yield return new WaitForSeconds(fallTime);
        isFalling = false;
        yield return new WaitForSeconds(1f); // Durée de l'animation
        canAttack = true;
        canMove = true;
        if (hitZoneVFX != null)
        {
            hitZoneVFX.gameObject.SetActive(false);
        }
    }

    #endregion

    public void DoDamage()
    {
        if (distanceToPlayer < attackRange + 0.02)
        {
            player.GetComponent<PlayerManager>().GetHurt(attackDamage);
            Vector3 dir = player.transform.position - transform.position;
            dir = new Vector3(dir.x, 0, dir.z).normalized * knockbackForce;
            player.GetComponent<PlayerManager>().rb.AddForce(dir, ForceMode.Impulse);
        }
    }

    public virtual void GetHurt(int damage)
    {
        if (isDead) return;
        if (isInvincible) return;

        currentHealth -= damage;

        isHurt = true;
        hurtAnim = true;
        hurtTime = Time.time;

        if (hurtVFX != null) hurtVFX.Play();

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    public void SetSpawnPoint(SpawnArea spawnArea)
    {
        spawnPoint = spawnArea;
    }

    public void Death()
    {
        rb.velocity = Vector3.zero;
        isDead = true;
        modelAggroMat.SetFloat("_Destruction", 1);
        if (doorIfDead) doorIfDead.keysValid++;
        if (currentArena) currentArena.currentSpawned.Remove(gameObject);

        Disable();

        GetComponent<CapsuleCollider>().isTrigger = true;

        if (GetComponentInChildren<CapsuleCollider>())
        {
            GetComponentInChildren<CapsuleCollider>().isTrigger = true;
        }

        if (deathVFX != null) deathVFX.Play();
        
        if (explosionVFX != null && !explodeOnEvent) explosionVFX.Play();

        GameManager.instance.enemyList.Remove(this);

        StartCoroutine(WaitForDestroy());
    }

    public void DropLoot()
    {
        int i = Random.Range(0, 101);
        if (i <= dropRate)
        {
            GameManager.instance.DropItem("Health", transform);
        }
    }

    IEnumerator WaitForDestroy()
    {
        DropLoot();
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);
    }

    public void LoadVFX(ParticleSystem effect)
    {
        effect.Play();
    }

    public void Enable()
    {
        isEnable = true;
        isHurt = false;
        canMove = true;
        canAttack = true;
    }

    public void Disable()
    {
        isEnable = false;
        if (nav.enabled) nav.SetDestination(transform.position);
        rb.velocity = Vector3.zero;
    }

    #region INSPECTOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dectectionRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    #endregion
}