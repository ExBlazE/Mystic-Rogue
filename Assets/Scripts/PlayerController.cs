using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Aiming")]
    [SerializeField] GameObject orbFocus;
    [SerializeField] GameObject orbObject;

    [Header("Movement")]
    [SerializeField] Animator playerAnim;
    [SerializeField] float moveSpeed = 5f;

    [Header("Shooting")]
    [SerializeField] ObjectPooler projectilePool;
    [SerializeField] float shotCooldown = 0.3f;
    private float currentShotCooldown = 0f;

    [Header("Shield")]
    [SerializeField] Shield shield;

    private InputHandler user;
    private Rigidbody playerRb;

    void Awake()
    {
        // Get reference to other components
        user = GetComponent<InputHandler>();
        playerRb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Reset cooldown
        currentShotCooldown = 0;
    }

    void Update()
    {
        // Set animator parameters
        SetMoveAnimParam();

        // Main player control logic to only run when game is active and not paused
        if (GameManager.Instance.gameState == GameState.Playing)
        {
            // Rotate orb to face mouse position
            MoveOrb();

            // Shoot projectile on left-click and when cooldown is zero
            if (user.IsShooting && currentShotCooldown == 0)
            {
                ShootProjectile();
            }

            // Reduce shot cooldown to zero
            if (currentShotCooldown > 0)
            {
                currentShotCooldown -= Time.deltaTime;
                if (currentShotCooldown < 0)
                    currentShotCooldown = 0;
            }

            // Enable shield when right-click is clicked
            if (user.IsShieldStarted)
            {
                if (!shield.isShieldActive)
                {
                    shield.Toggle(true);
                }
            }

            // Disable shield when right-click is released
            if (user.IsShieldReleased && shield.isShieldActive)
            {
                shield.Toggle(false);
            }
        }
    }

    // Move logic in FixedUpdate for smooth rigidbody physics
    void FixedUpdate()
    {
        Move();
    }

    // Method to move player
    void Move()
    {
        Vector3 movement = new Vector3(user.MoveDirection.x, 0, user.MoveDirection.y);

        // Clamp direction vector magnitude at 1 to prevent faster diagonal speed
        movement = Vector3.ClampMagnitude(movement, 1.0f);

        // Move player (via physics movement)
        playerRb.linearVelocity = movement * moveSpeed;
    }

    // Method to spin orb around player
    void MoveOrb()
    {
        if (user.AimDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(user.AimDirection);
            orbFocus.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        }
    }

    // Method to set animator parameters according to player velocity
    void SetMoveAnimParam()
    {
        // Get world space velocity of orb focus from its rigidbody
        Vector3 worldVelocity = playerRb.linearVelocity;

        // Convert to local space velocity
        Vector3 localVelocity = orbFocus.transform.InverseTransformDirection(worldVelocity);

        // Set animator parameters accordingly
        playerAnim.SetFloat("f_moveForward", localVelocity.z);
        playerAnim.SetFloat("f_moveRight", localVelocity.x);
    }

    private void ShootProjectile()
    {
        // Get the spawn position and rotation
        Vector3 spawnPos = orbObject.transform.position;
        Quaternion spawnRot = orbFocus.transform.rotation;

        // Spawn the projectile and start cooldown
        GameObject projectile = projectilePool.GetFromPool();
        projectile.transform.position = spawnPos;
        projectile.transform.rotation = spawnRot;
        currentShotCooldown = shotCooldown;

        // Fire an event to indicate projectile fired
        GameEvents.RaiseOnShotFired();
    }
}
