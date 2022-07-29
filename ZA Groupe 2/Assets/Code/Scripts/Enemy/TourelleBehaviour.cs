using UnityEngine;

public class TourelleBehaviour : MonoBehaviour
{
    private Transform player;

    [SerializeField] private Animator anim;
    [SerializeField] private Transform canon;
    [SerializeField] private Transform followTransform;

    [SerializeField] private Transform bulletOrigin;

    [SerializeField] private TourelleState currentState;

    [SerializeField] private float detectionRange;

    [SerializeField] private float detectionDuration;
    private float detectionTimer;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private float followDuration;
    private float followTimer;

    [SerializeField] private float beforeShootDuration;
    private float beforeShootTimer;

    public bulletBehavior bullet;
    public bulletBehavior currentBullet;
    
    [SerializeField] private bool isInvincible;
    [SerializeField] private bool constantFire;
    [SerializeField] private Door doorOnDeath;

    [SerializeField] private float deathDuration;
    private float deathTimer;

    [SerializeField] private float cooldownDuration;
    private float cooldownTimer;

    public Material modelNonAggroMat;
    public AnimationCurve animationDeath;
    public float hurtTime;

    [Header("VFX")]
    [SerializeField] private ParticleSystem shootVFX;
    [SerializeField] private ParticleSystem detectionVFX;
    [SerializeField] private ParticleSystem shieldVFX;
    [SerializeField] private ParticleSystem deathVFX;
    [SerializeField] private GameObject targetRay;

    [SerializeField] private Material targetMat;
    [SerializeField] private MeshRenderer targetRenderer;
    [SerializeField] private AnimationCurve targetSpeed;

    public enum TourelleState
    {
        Idle,
        Detection,
        Follow,
        Shoot,
        Destroy,
        Cooldown
    }

    private void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        player = PlayerManager.instance.transform;
        SwitchState(TourelleState.Idle);
    }

    private void Update()
    {
        CheckState();
    }

    private void CheckState()
    {
        switch (currentState)
        {
            case TourelleState.Idle:

                var distance = Vector3.Distance(player.position, transform.position);
                if (distance <= detectionRange) SwitchState(TourelleState.Detection);

                break;

            case TourelleState.Detection:

                if (detectionTimer >= detectionDuration) SwitchState(TourelleState.Follow);
                else detectionTimer += Time.deltaTime;

                break;

            case TourelleState.Follow:

                canon.rotation = Quaternion.Lerp(canon.rotation,
                    Quaternion.LookRotation(-(transform.position - player.position)),
                    Time.deltaTime * rotationSpeed);
                canon.eulerAngles = new Vector3(0, canon.eulerAngles.y, 0);

                if (currentBullet && !constantFire) return;

                if (followTimer >= followDuration) SwitchState(TourelleState.Shoot);
                else
                {
                    targetRenderer.material.SetFloat("_BlinkSpeed", targetSpeed.Evaluate(followTimer));
                    followTimer += Time.deltaTime;
                }

                break;

            case TourelleState.Shoot:

                if (beforeShootTimer >= beforeShootDuration)
                {
                    shootVFX.Play();
                    currentBullet = Instantiate(bullet, bulletOrigin.position, Quaternion.identity);
                    currentBullet.velocity = canon.forward;

                    SwitchState(TourelleState.Cooldown);
                }
                else beforeShootTimer += Time.deltaTime;

                break;

            case TourelleState.Destroy:
                
                //modelNonAggroMat.SetFloat("_NoiseStrenght", animationDeath.Evaluate(Time.time - hurtTime));
                
                if (deathTimer >= deathDuration)
                {
                    if (doorOnDeath) doorOnDeath.keysValid++;
                    Destroy(gameObject);
                }
                else deathTimer += Time.deltaTime;

                break;
            
            case TourelleState.Cooldown:
                
                if (cooldownTimer >= cooldownDuration)
                {
                    SwitchState(TourelleState.Idle);
                }
                else cooldownTimer += Time.deltaTime;

                break;
        }
    }

    private void SwitchState(TourelleState state)
    {
        switch (state)
        {
            case TourelleState.Idle:
                anim.enabled = false;

                break;

            case TourelleState.Detection:
                anim.enabled = true;
                anim.SetBool("Shooting", false);
                if (!detectionVFX.isPlaying) detectionVFX.Play();
                targetRay.SetActive(true);
                detectionTimer = 0f;
                break;

            case TourelleState.Follow:
                anim.enabled = true;
                anim.SetBool("Shooting", false);

                followTimer = 0f;
                break;

            case TourelleState.Shoot:
                anim.enabled = true;
                anim.SetBool("Shooting", true);
                targetRay.SetActive(false);
                beforeShootTimer = 0f;
                //targetRenderer.material.SetFloat("_BlinkSpeed", 50f);

                break;

            case TourelleState.Destroy:
                anim.enabled = false;
                anim.SetBool("Shooting", false);
                deathVFX.Play();
                targetRay.SetActive(false);
                //modelNonAggroMat.SetFloat("_Destruction", 1);

                deathTimer = 0f;
                break;
            
            case TourelleState.Cooldown:
                anim.enabled = true;
                anim.SetBool("Shooting", false);

                cooldownTimer = 0f;
                
                break;
        }

        currentState = state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (isInvincible)
            {
                OnInvincibleTourelleHit();
                return;
            }

            SwitchState(TourelleState.Destroy);
        }
    }

    private void OnInvincibleTourelleHit()
    {
        shieldVFX.Play();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}