using UnityEngine;

/// <summary>
/// Manages projectile life-cycle and collision behaviour.
/// </summary>
public class Projectile : MonoBehaviour
{
    [SerializeField] bool isPlayerSide;

    [Space]
    [SerializeField] float damage = 10;
    [SerializeField] float speed = 15f;
    [SerializeField] float maxDuration = 3f;

    private float aliveDuration;
    private bool hasCollided = false;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Initialization logic in OnEnable because this object will be in object pool
    // And thus needs to initialize multiple times in a single game object life-cycle
    void OnEnable()
    {
        aliveDuration = maxDuration;
        hasCollided = false;
        rb.linearVelocity = transform.forward * speed;
    }

    // Safety reset just in case
    void OnDisable()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void Update()
    {
        // Disable projectile after set duration
        if (aliveDuration > 0)
            aliveDuration -= Time.deltaTime;
        else
            gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        // This block is to prevent double collision cases
        if (hasCollided)
            return;

        if (other.TryGetComponent<IDamageable>(out var target))
        {
            bool isFriendlyFire = (this.isPlayerSide == target.IsPlayerSide);
            if (!isFriendlyFire)
            {
                hasCollided = true;
                target.OnHit(damage);
                GameEvents.RaiseOnShotHit(transform.position, transform.rotation, target.TargetType);
                gameObject.SetActive(false);
            }
        }
    }
}
