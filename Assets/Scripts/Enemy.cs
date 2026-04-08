using UnityEngine;

/// <summary>
/// Basic class for enemy AI.
/// </summary>
public class Enemy : MonoBehaviour, IDamageable
{
    public bool IsPlayerSide => false;
    public Target TargetType => Target.Enemy;

    public bool canMove = true;
    public bool canShoot = true;

    [Space]
    [SerializeField] float moveSpeed = 2.5f;
    [SerializeField] float shotRange = 15f;
    [SerializeField] float shotCooldown = 2.5f;
    [SerializeField] float collisionDamage = 20f;

    [Space]
    [SerializeField] ObjectPooler projectilePool;

    [Space]
    [SerializeField] GameObject focus;
    [SerializeField] Animator enemyAnim;

    private float shotRangeSqr;
    private float currentCooldown = 0f;

    private Player playerRef;
    private Rigidbody enemyRb;
    
    void Awake()
    {
        // Get reference to rigidbody component
        enemyRb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Square the shot range distance and reset cooldown
        shotRangeSqr = shotRange * shotRange;
        currentCooldown = 0f;
    }

    void Update()
    {
        // Calculate distance-squared to player
        float distanceToPlayerSqr = (playerRef.transform.position - transform.position).sqrMagnitude;

        // If within range and cooldown is zero, shoot projectile
        // We compare square of both numbers because Vector3.magnitude takes more time for CPU
        if (canShoot && currentCooldown == 0 && distanceToPlayerSqr < shotRangeSqr)
        {
            ShootProjectile();
        }

        // Reduce cooldown to zero
        if (currentCooldown > 0)
            currentCooldown -= Time.deltaTime;
        if (currentCooldown < 0)
            currentCooldown = 0;

        // Set movement animation parameters
        enemyAnim.SetFloat("f_speed", enemyRb.linearVelocity.magnitude);
    }

    // Move logic in FixedUpdate for smooth collision physics
    void FixedUpdate()
    {
        MoveTowardsPlayer();
    }

    // Method to move enemy towards player's position
    void MoveTowardsPlayer()
    {
        // If movement is enabled in inspector
        if (canMove)
        {
            // Get direction of player from enemy position
            Vector3 direction = playerRef.transform.position - enemyRb.position;

            // Create new rotation facing player and apply it to enemy
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

            // Move enemy towards player (physics method)
            enemyRb.linearVelocity = enemyRb.transform.forward * moveSpeed;
        }

        // If movement is disabled in inspector
        else
        {
            enemyRb.linearVelocity = Vector3.zero;
        }
    }

    // Method to shoot projectile
    void ShootProjectile()
    {
        // Get spawn position and rotation
        Vector3 spawnPos = focus.transform.position;
        Quaternion spawnRot = transform.rotation;

        // Spawn projectile, launch it and start cooldown
        projectilePool.GetFromPool(spawnPos, spawnRot);
        currentCooldown = shotCooldown;
    }

    // Logic to handle enemy touching player
    void OnCollisionEnter(Collision collision)
    {
        bool isDamageable = collision.gameObject.TryGetComponent<IDamageable>(out var target);

        if (!isDamageable) return;

        bool isPlayerSide = target.IsPlayerSide;
        bool isPlayer = (target.TargetType == Target.Player);

        if (isDamageable && isPlayerSide && isPlayer)
        {
            target.OnHit(collisionDamage);
            this.OnHit();

            GameEvents.RaiseOnShotHit(focus.transform.position, transform.rotation, Target.Player);
        }
    }

    // Logic to handle enemy touching shield
    void OnTriggerEnter(Collider other)
    {
        bool isDamageable = other.TryGetComponent<IDamageable>(out var target);

        if (!isDamageable)
            return;

        bool isPlayerSide = target.IsPlayerSide;
        bool isShield = (target.TargetType == Target.Shield);

        if (isDamageable && isPlayerSide && isShield)
        {
            target.OnHit(collisionDamage);
            this.OnHit();

            GameEvents.RaiseOnShotHit(focus.transform.position, transform.rotation, Target.Shield);
        }
    }

    public void OnHit(float damage = default)
    {
        gameObject.SetActive(false);
        GameEvents.RaiseOnEnemyDeath(1);
    }

    // For dependency injection by SpawnManager
    public void Initialize(Player playerRef, ObjectPooler projectilePool)
    {
        this.playerRef = playerRef;
        this.projectilePool = projectilePool;
    }
}
