using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("Aiming")]
    [SerializeField] GameObject orbFocus;
    [SerializeField] GameObject orbObject;
    [SerializeField] LayerMask targetLayers;

    [Header("Movement")]
    [SerializeField] Animator playerAnim;
    [SerializeField] float moveSpeed = 1f;

    [Header("Shooting")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float shotCooldown = 0.3f;

    private float currentShotCooldown = 0f;

    [Header("Shield")]
    [SerializeField] GameObject shieldObject;
    [SerializeField] float shieldRegenPerSecond = 10f;
    [SerializeField] float shieldCostPerSecond = 5f;
    [SerializeField] float shieldRegenCooldown = 3f;
    [SerializeField] float shieldFadeDuration = 0.1f;

    private float currentShieldRegenCooldown;
    private bool isShieldActive;
    private bool shieldFading = false;
    private bool fadeOutQueued = false;
    private Renderer shieldRenderer;
    private const string shieldAlphaName = "_Alpha";

    [Header("Stats")]
    public float maxHealth = 100f;
    public float maxShield = 50f;

    public float health { get; private set; }
    public float shield { get; private set; }

    private Rigidbody playerRb;

    private float verticalInput;
    private float horizontalInput;

    public static PlayerControl Instance;

    void Awake()
    {
        // Create singleton reference to this script
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Get reference to player rigidbody component
        playerRb = GetComponent<Rigidbody>();

        // Set starting health and shield to max
        health = maxHealth;
        shield = maxShield;
    }

    void Start()
    {
        // Disable shield by default at start
        shieldObject.SetActive(false);
        isShieldActive = false;

        // Reset cooldowns
        currentShotCooldown = 0;
        currentShieldRegenCooldown = 0;

        // Get shield's renderer component
        shieldRenderer = shieldObject.GetComponent<Renderer>();
    }

    void Update()
    {
        // Get player input via WASD (used in FixedUpdate)
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        // Set animator parameters
        SetMoveAnimParam();

        // Main player control logic to only run when game is active and not paused
        if (GameManager.Instance.isGameActive && !GameManager.Instance.isGamePaused)
        {
            // Rotate orb to face mouse position
            MoveOrb();

            // Shoot projectile on left-click and when cooldown is zero
            if (Input.GetKeyDown(KeyCode.Mouse0) && currentShotCooldown == 0)
            {
                ShootProjectile();
            }

            // Reduce shot cooldown to zero
            if (currentShotCooldown > 0)
            {
                ReduceCooldown(ref currentShotCooldown);
            }

            // Enable shield when right-click is clicked
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (!isShieldActive && shield > 0 && !shieldFading)
                {
                    StartCoroutine(ShieldEnabled(true));
                    currentShieldRegenCooldown = shieldRegenCooldown;

                    AudioManager.Instance.PlayShield();
                }
            }

            // Disable shield when right-click is released
            if (Input.GetKeyUp(KeyCode.Mouse1) && isShieldActive)
            {
                StartCoroutine(ShieldEnabled(false));
            }

            // Disable shield when zero
            if (shield == 0 && isShieldActive)
            {
                StartCoroutine(ShieldEnabled(false));
            }

            // Decrease shield value when active
            if (isShieldActive)
            {
                float shieldReduce = shieldCostPerSecond * Time.deltaTime;
                ModifyShield(-shieldReduce);
            }

            // Reduce shield regen cooldown to zero
            if (currentShieldRegenCooldown > 0 && !isShieldActive)
            {
                ReduceCooldown(ref currentShieldRegenCooldown);
            }

            // Regen shield
            if (shield < maxShield && currentShieldRegenCooldown == 0 && !isShieldActive)
            {
                float shieldRegen = shieldRegenPerSecond * Time.deltaTime;
                ModifyShield(shieldRegen);
            }

            // Synchronize shield's active state with bool
            shieldObject.SetActive(isShieldActive);

            // End game when health is zero
            if (health == 0)
            {
                GameManager.Instance.EndGame();
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
        // Create direction vector based on player input
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);

        // Clamp direction vector magnitude at 1 to prevent faster diagonal speed
        movement = Vector3.ClampMagnitude(movement, 1.0f);

        // Move player (via physics movement)
        playerRb.linearVelocity = movement * moveSpeed;
    }

    // Method to spin orb around player to always point at mouse position
    void MoveOrb()
    {
        // Draw a ray from camera through the mouse pointer
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Cast the ray until it hits an object in the selected layers
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayers))
        {
            // Get mouse position on ground level and direction from player
            Vector3 mousePosition = hit.point;
            Vector3 mouseDirection = mousePosition - orbFocus.transform.position;

            // Create new rotation facing mouse direction and apply it to orbFocus
            Quaternion targetRotation = Quaternion.LookRotation(mouseDirection);
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
        // Get the spawn position, rotation and object transform to parent projectile to
        Vector3 spawnPos = orbObject.transform.position;
        Quaternion spawnRot = orbFocus.transform.rotation;
        Transform spawnParent = GameManager.Instance.projectileGroupObject;

        // Spawn the projectile and start cooldown
        Instantiate(projectilePrefab, spawnPos, spawnRot, spawnParent);
        currentShotCooldown = shotCooldown;

        // Play shooting sound
        AudioManager.Instance.PlayShotStart();
    }

    // Method to add or remove health
    public void ModifyHealth(float changeHealth)
    {
        health += changeHealth;

        // Restrict value between zero and max
        if (health < 0)
            health = 0;
        else if (health > maxHealth)
            health = maxHealth;
    }

    // Method to add or remove shield
    public void ModifyShield(float changeShield)
    {
        shield += changeShield;

        // Restrict value between zero and max
        if (shield < 0)
            shield = 0;
        else if (shield > maxShield)
            shield = maxShield;
    }

    // Method to reduce cooldown timer to zero
    // Passed parameter directly takes given variable instead of its value
    private void ReduceCooldown(ref float timer)
    {
        timer -= Time.deltaTime;
        if (timer < 0)
            timer = 0;
    }

    // Coroutine to enable or disable shield
    IEnumerator ShieldEnabled(bool enabled)
    {
        // This block is meant to queue up a single fade-out after a fade-in
        // Runs only if right-click is released too quickly
        if (shieldFading)
        {
            if (!fadeOutQueued)
            {
                fadeOutQueued = true;
                while (shieldFading)
                    yield return null;
            }
            else
                yield break;
        }

        // Set flag to indicate shield is fading
        shieldFading = true;

        float startAlpha;
        float endAlpha;

        float timeElapsed = 0f;

        // Set start and end values of alpha for fading in
        if (enabled)
        {
            startAlpha = 0f;
            endAlpha = 1f;

            // Activate the shield game object
            isShieldActive = true;
        }

        // Set start and end values of alpha for fading out
        // We don't deactivate shield here because it's done after the fade out animation
        else
        {
            startAlpha = 1f;
            endAlpha = 0f;
        }        

        // Set gradually increasing or decrease alpha value per frame until end value
        while (timeElapsed < shieldFadeDuration)
        {
            // Calculate the new alpha value for this frame
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, timeElapsed / shieldFadeDuration);

            // Clamp the min and max values of alpha
            if (newAlpha < 0f)
                newAlpha = 0f;
            else if (newAlpha > 1f)
                newAlpha = 1f;

            // Set the new alpha value for this frame
            shieldRenderer.material.SetFloat(shieldAlphaName, newAlpha);

            // Increase progress indicator of the fade for next rerun of the loop
            timeElapsed += Time.deltaTime;

            // Pause the coroutine until next frame
            yield return null;
        }

        // If fading out, deactivate shield object and reset queue flag
        if (!enabled)
        {
            isShieldActive = false;
            fadeOutQueued = false;
        }

        // Set flag to indicate that fading is complete
        shieldFading = false;
    }
}
