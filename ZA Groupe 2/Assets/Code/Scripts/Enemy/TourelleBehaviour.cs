using UnityEngine;

public class TourelleBehaviour : MonoBehaviour
{
    private Transform player;
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

    [SerializeField] private ParticleSystem shootVFX;
    [SerializeField] private ParticleSystem detectionVFX;

    [SerializeField] private bool isInvincible;

    [SerializeField] private float deathDuration;
    private float deathTimer;

    public enum TourelleState
    {
        Idle,
        Detection,
        Follow,
        Shoot,
        Destroy
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

                if (currentBullet) return;

                if (followTimer >= followDuration) SwitchState(TourelleState.Shoot);
                else followTimer += Time.deltaTime;

                break;

            case TourelleState.Shoot:

                if (beforeShootTimer >= beforeShootDuration)
                {
                    shootVFX.Play();
                    currentBullet = Instantiate(bullet, bulletOrigin.position, Quaternion.identity);
                    currentBullet.velocity = canon.forward;

                    SwitchState(TourelleState.Idle);
                }
                else beforeShootTimer += Time.deltaTime;

                break;

            case TourelleState.Destroy:

                if (deathTimer >= deathDuration)
                {
                    Destroy(gameObject);
                }
                else deathTimer += Time.deltaTime;

                break;
        }
    }

    private void SwitchState(TourelleState state)
    {
        switch (state)
        {
            case TourelleState.Idle:
                // Feedback quand passe en idle
                break;

            case TourelleState.Detection:

                if (!detectionVFX.isPlaying) detectionVFX.Play();
                detectionTimer = 0f;
                break;

            case TourelleState.Follow:
                // Feedback quand commence à viser

                followTimer = 0f;
                break;

            case TourelleState.Shoot:
                // Feedback quand se bloque avant de tirer

                break;

            case TourelleState.Destroy:

                // Death feedback
                // Anim fade
                
                deathTimer = 0f;
                break;
        }

        currentState = state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") && !isInvincible)
        {
            SwitchState(TourelleState.Destroy);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}