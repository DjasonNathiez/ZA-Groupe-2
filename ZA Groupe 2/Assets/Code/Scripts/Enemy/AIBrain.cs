using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Random = UnityEngine.Random;

public class AIBrain : MonoBehaviour
{
    [HideInInspector] public SpawnArea spawnPoint;

    [HideInInspector] public Rigidbody rb;
    public GameObject player;
    public NavMeshAgent nav;
    [HideInInspector] public Animator animator;
    [HideInInspector] public float distanceToPlayer;

    [HideInInspector] public bool isEnable = true;

    [Header("State")]
    public int currentHealth;
    public int maxHealth;
    [HideInInspector] public bool isInvincible;
    
    public float fallTime;
    [HideInInspector] public float timeOnGround;
   
    public bool canFall;
    public bool canMove = true;
    public bool canAttack = true;
    public bool canBeKnocked;
    public bool counterState;
    
    [HideInInspector] public bool isFalling;
    public bool isAttacking;
    public bool isAggro;
    [HideInInspector] public bool isKnocked;
    public bool isMoving;
     public bool isDead;
    public bool isHurt;
    
    [Header("Movement")]
    public float moveSpeed;
    
    [Header("Attack")]
    public int attackDamage;
    public float attackRange;
    public float attackSpeed; 
    public float attackDelay;
    [HideInInspector] public float activeAttackCd;
    [HideInInspector] public bool attackOnCd;
    public float knockbackForce;
    
    [Header("Detection")]
    public float dectectionRange;
    public float massAggroRange;
    [Range(0,180)] public float detectionAngle;
    public Door doorIfDead;
    public ArenaParc currentArena;
    [HideInInspector] public bool playerShowBack;
    
    [Header("VFX")]
    public ParticleSystem hurtVFX;
    public ParticleSystem attackVFX;
    public ParticleSystem hitZoneVFX;
    public ParticleSystem deathVFX;
    public ParticleSystem explosionVFX;
    public bool explodeOnEvent;
    public ParticleSystem aggroVFX;
    public ParticleSystem counterVFX;

    [Header("Visual")] 
    public List<SkinnedMeshRenderer> modelMeshRenderer;
    public Material modelNonAggroMat;
    public Material modelAggroMat;
    public GameObject enemyStatusPointer;
    public Material aggroMaterial;
    public Material nonAggroMaterial;
    public AnimationCurve animationHurt;
    public AnimationCurve animationDeath;
    public bool hurtAnim;
    public float hurtTime;

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
        player = GameManager.instance.player;
        nav = GetComponent<NavMeshAgent>();
        
        nav.speed = moveSpeed;
        nav.stoppingDistance = attackRange + 0.02f;
    }

    public void MoveToPlayer(Vector3 destination)
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
                enemyStatusPointer.GetComponent<MeshRenderer>().material = aggroMaterial;
            }
            else
            {
                mesh.material = modelNonAggroMat;
                enemyStatusPointer.GetComponent<MeshRenderer>().material = nonAggroMaterial;
            }
        }
    }
    
    public void Detection()
    {
        //Detect the player to aggro
        if (player == null) return;
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= dectectionRange && !isAggro)
        {
            Collider[] hit = Physics.OverlapSphere(transform.position, dectectionRange);
        
            foreach (Collider col in hit)
            {
                if (col.GetComponent<PlayerManager>())
                {
                    if (GetComponent<RabbitBehaviour>() == null)
                    {
                        isAggro = true;
                        
                        if (aggroVFX != null)
                        {
                            aggroVFX.Play();
                        }

                        if (GetComponent<LionBehaviour>())
                        {
                            GetComponent<LionBehaviour>().PlaySFX("L_Aggro");
                        }

                        if (GetComponent<BearBehaviour>())
                        {
                            GetComponent<BearBehaviour>().PlaySFX("B_Aggro");
                        }
                    }
                
                }
            }
        }
        

        //Mass Aggro AI Comportement
        if (isAggro)
        {
            Collider[] hitMass = Physics.OverlapSphere(transform.position, massAggroRange);

            foreach (Collider c in hitMass)
            {
                AIBrain neighborAI = c.GetComponent<AIBrain>();
                
                if (neighborAI && !neighborAI.isAggro)
                {
                    neighborAI.isAggro = true;
                }
            }
        }

        
        float directionAngle = Vector3.Angle(player.transform.forward, transform.forward);

        playerShowBack = directionAngle < detectionAngle;

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
        if (attackVFX != null)
        {
            attackVFX.Play();
        }
    }

    public void AttackExit()
    {
        isAttacking = false;
        canMove = true;
    }
    
    public void FallOnTheGround()
    {
        //Set State
        isFalling = true;
        isInvincible = false;
        isAttacking = false;
        canAttack = false;
        canMove = false;
        
        GetComponent<LionBehaviour>().standUpVFX.Play();
        
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


    public void GetHurt(int damage)
    {
        if (!isDead)
        {
            if (counterState)
            {
               counterVFX.Play();
            }
            
            if (isInvincible) return;
            
            currentHealth -= damage;
            
            isHurt = true;
            hurtAnim = true;
            hurtTime = Time.time;
            
            if (hurtVFX != null)
            {
                hurtVFX.Play();
            }

            if (currentHealth <= 0)
            {
                Death();
            }
        }
    }

    public void SetSpawnPoint(SpawnArea spawnArea)
    {
        spawnPoint = spawnArea;
    }

    public void Death()
    {
        isDead = true;
        modelAggroMat.SetFloat("_Destruction",1);
        if (doorIfDead) doorIfDead.keysValid++;
        if (currentArena) currentArena.currentSpawned.Remove(this.gameObject);
        
        Disable();
        
        GetComponent<CapsuleCollider>().isTrigger = true;
        
        if (GetComponentInChildren<CapsuleCollider>())
        {
            GetComponentInChildren<CapsuleCollider>().isTrigger = true;
        }

        if (deathVFX != null)
        {
            deathVFX.Play();
        }

        if (explosionVFX != null && !explodeOnEvent)
        {
            explosionVFX.Play();
        }
        
        GameManager.instance.enemyList.Remove(this);
        
        StartCoroutine(WaitForDestroy());
    }

    void DropLoot()
    {
        int i = Random.Range(0, 100);
        
        if (GetComponent<LionBehaviour>())
        {
            if (i >= 77)
            {
                GameManager.instance.DropItem("Health", transform);
            }
        }

        if (GetComponent<BearBehaviour>())
        {
            if (i >= 50)
            {
                GameManager.instance.DropItem("Health", transform);
            }
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
        if(nav.enabled) nav.SetDestination(transform.position);
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

