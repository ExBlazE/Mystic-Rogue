using UnityEngine;

/// <summary>
/// Script for controlling the player.<br/>
/// <i>MANDATORY for every scene with controllable player.</i>
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Aiming")]
    [SerializeField] GameObject orbFocus;
    [SerializeField] GameObject orbObject;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Animator playerAnim;

    [Header("Shooting")]
    [SerializeField] float shotCooldown = 0.3f;
    [SerializeField] ObjectPooler projectilePool;

    [Header("Shield")]
    [SerializeField] Shield shield;

    private float currentShotCooldown = 0f;
    private Vector2 currentMoveDirection = Vector2.zero;

    private Rigidbody playerRb;

    void OnEnable()
    {
        InputEvents.OnMove += HandleMove;
        InputEvents.OnAim += RotatePlayer;
        InputEvents.OnShoot += ShootProjectile;
        InputEvents.OnShield += HandleShield;

        GameEvents.OnGameResume += ForceDisableShield;
    }

    void OnDisable()
    {
        InputEvents.OnMove -= HandleMove;
        InputEvents.OnAim -= RotatePlayer;
        InputEvents.OnShoot -= ShootProjectile;
        InputEvents.OnShield -= HandleShield;

        GameEvents.OnGameResume -= ForceDisableShield;
    }

    void Awake()
    {
        // Get reference to other components
        playerRb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Reset cooldown
        currentShotCooldown = 0;
    }

    void Update()
    {
        // Guard clause to ignore following code if game is paused or over
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
        MovePlayer();
    }

    // Receive latest movement input data via event
    void HandleMove(Vector2 moveDirection)
    { currentMoveDirection = moveDirection; }

    // Moves player according to latest movement data
    void MovePlayer()
    {
        if (GameManager.Instance.gameState != GameState.Playing) return;

        Vector3 movement = new Vector3(currentMoveDirection.x, 0, currentMoveDirection.y);

        // Clamp direction vector magnitude at 1 to prevent faster diagonal speed
        movement = Vector3.ClampMagnitude(movement, 1.0f);

        // Move player (via physics movement)
        playerRb.linearVelocity = movement * moveSpeed;
    }

    // Method to rotate player
    void RotatePlayer(Vector3 aimDirection)
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

        // Spawn the projectile, launch it and start cooldown
        projectilePool.GetFromPool(spawnPos, spawnRot);
        currentShotCooldown = shotCooldown;

        // Fire an event to indicate projectile fired
        GameEvents.RaiseOnShotFired();
    }

    void HandleShield(bool shieldEnable)
    {
        if (GameManager.Instance.gameState != GameState.Playing) return;

        if (shieldEnable)
        {
            if (!shield.IsShieldActive)
                shield.Toggle(true);
        }
        else
        {
            if (shield.IsShieldActive)
                shield.Toggle(false);
        }
    }

    void ForceDisableShield()
    { HandleShield(false); }
}
