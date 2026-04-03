using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Aiming")]
    [SerializeField] GameObject orbFocus;
    [SerializeField] GameObject orbObject;

    [Header("Movement")]
    [SerializeField] Animator playerAnim;
    [SerializeField] float moveSpeed = 5f;
    private Vector2 currentMoveDirection = Vector2.zero;

    [Header("Shooting")]
    [SerializeField] ObjectPooler projectilePool;
    [SerializeField] float shotCooldown = 0.3f;
    private float currentShotCooldown = 0f;

    [Header("Shield")]
    [SerializeField] Shield shield;

    private InputHandler input;
    private Rigidbody playerRb;

    void OnEnable()
    {
        input.OnMove += HandleMove;
        input.OnAim += MoveOrb;
        input.OnShoot += ShootProjectile;
        input.OnShield += HandleShield;

        GameEvents.OnGameResume += ForceDisableShield;
    }

    void OnDisable()
    {
        input.OnMove -= HandleMove;
        input.OnAim -= MoveOrb;
        input.OnShoot -= ShootProjectile;
        input.OnShield -= HandleShield;

        GameEvents.OnGameResume -= ForceDisableShield;
    }

    void Awake()
    {
        // Get reference to other components
        input = GetComponent<InputHandler>();
        playerRb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Reset cooldown
        currentShotCooldown = 0;
    }

    void Update()
    {
        if (GameManager.Instance.gameState != GameState.Playing) return;

        // Set animator parameters
        SetMoveAnimParam();

        // Reduce shot cooldown to zero
        if (currentShotCooldown > 0)
        {
            currentShotCooldown -= Time.deltaTime;
            if (currentShotCooldown < 0)
                currentShotCooldown = 0;
        }
    }

    // Move logic in FixedUpdate for smooth rigidbody physics
    void FixedUpdate()
    {
        Move();
    }

    void HandleMove(Vector2 moveDirection)
    { currentMoveDirection = moveDirection; }

    // Method to move player
    void Move()
    {
        if (GameManager.Instance.gameState != GameState.Playing) return;

        Vector3 movement = new Vector3(currentMoveDirection.x, 0, currentMoveDirection.y);

        // Clamp direction vector magnitude at 1 to prevent faster diagonal speed
        movement = Vector3.ClampMagnitude(movement, 1.0f);

        // Move player (via physics movement)
        playerRb.linearVelocity = movement * moveSpeed;
    }

    // Method to spin orb around player
    void MoveOrb(Vector3 aimDirection)
    {
        if (GameManager.Instance.gameState != GameState.Playing) return;

        if (aimDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
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

    void ShootProjectile()
    {
        if (GameManager.Instance.gameState != GameState.Playing) return;
        if (currentShotCooldown != 0) return;

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

    void HandleShield(bool shieldEnable)
    {
        if (GameManager.Instance.gameState != GameState.Playing) return;

        if (shieldEnable)
        {
            if (!shield.isShieldActive)
                shield.Toggle(true);
        }
        else
        {
            if (shield.isShieldActive)
                shield.Toggle(false);
        }
    }

    void ForceDisableShield()
    { HandleShield(false); }
}
