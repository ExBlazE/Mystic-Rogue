using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject focus;
    public bool canMove = true;
    [SerializeField] float moveSpeed = 2.5f;

    [Space]
    [SerializeField] GameObject projectilePrefab;
    public bool canShoot = true;
    [SerializeField] float shotRange = 15f;
    [SerializeField] float shotCooldown = 2.5f;
    [SerializeField] float collisionDamage = 20f;

    private float currentCooldown = 0f;

    [Space]
    [SerializeField] Animator enemyAnim;

    [Space]
    [SerializeField] ParticleSystem playerCollideFX;
    [SerializeField] ParticleSystem shieldCollideFX;

    private Rigidbody enemyRb;

    PlayerControl player;
    GameManager gm;
    
    void Awake()
    {
        // Get reference to rigidbody component
        enemyRb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        player = PlayerControl.Instance;
        gm = GameManager.Instance;
    }

    void Update()
    {
        // Calculate square of shot range
        float shotRangeSqr = shotRange * shotRange;

        // Calculate distance-squared to player
        float distanceToPlayerSqr = (player.transform.position - transform.position).sqrMagnitude;

        // If within range and cooldown is zero, shoot projectile
        // We compare square of both numbers because Vector3.magnitude takes more time for CPU
        if (distanceToPlayerSqr < shotRangeSqr)
        {
            if (currentCooldown == 0 && canShoot)
            {
                ShootProjectile();
            }
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
            Vector3 direction = player.transform.position - enemyRb.position;

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
        // Get spawn position, rotation, parent transform
        Vector3 spawnPos = focus.transform.position;
        Quaternion spawnRot = transform.rotation;
        Transform spawnParent = gm.projectileGroup;

        // Spawn projectile and start cooldown
        Instantiate(projectilePrefab, spawnPos, spawnRot, spawnParent);
        currentCooldown = shotCooldown;
    }

    // Logic to handle enemy touching player
    void OnCollisionEnter(Collision collision)
    {
        // If touching player, deplete health
        if (collision.gameObject.CompareTag("Player"))
        {
            // Execute collision logic and reduce player health
            PlayCollisionFX(playerCollideFX);
            OnHit();
            player.ModifyHealth(-collisionDamage);
        }
    }

    // Logic to handle enemy touching shield
    void OnTriggerEnter(Collider other)
    {
        // If touching shield, deplete shield
        if (other.CompareTag("Shield"))
        {
            // Execute collision logic and reduce player shield
            PlayCollisionFX(shieldCollideFX);
            OnHit();
            player.ModifyShield(-collisionDamage);
        }
    }

    void PlayCollisionFX(ParticleSystem collisionParticles)
    {
        // Create hit effects
        Transform particlesGroup = gm.particlesGroup;
        Instantiate(collisionParticles, focus.transform.position, transform.rotation, particlesGroup);

        // Play collision sound
        AudioManager.Instance.PlayShotHit();
    }

    public void OnHit()
    {
        gameObject.SetActive(false);
        gm.enemiesOnScreen--;
        gm.AddScore(1);
    }
}
