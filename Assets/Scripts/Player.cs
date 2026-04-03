using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    public bool IsPlayerSide => true;
    public Target TargetType => Target.Player;

    [Header("Shield")]
    [SerializeField] Shield shield;
    [SerializeField] float shieldRegenPerSecond = 10f;
    [SerializeField] float shieldCostPerSecond = 5f;
    [SerializeField] float energyRegenCooldown = 3f;
    private float currentEnergyRegenCooldown;

    public float maxHealth { get; private set; } = 100f;
    public float maxEnergy { get; private set; } = 50f;
    public float health { get; private set; }
    public float energy { get; private set; }

    private bool isDead;

    void Awake()
    {
        // Set starting health and shield to max
        health = maxHealth;
        energy = maxEnergy;
        isDead = false;
    }

    void Start()
    {
        // Reset cooldowns
        currentEnergyRegenCooldown = 0;
    }

    void Update()
    {
        // Energy and shield logic to run only when game is active and unpaused
        if (GameManager.Instance.gameState == GameState.Playing)
        {
            // Disable shield when zero
            if (energy == 0 && shield.isShieldActive)
            {
                shield.Toggle(false);
            }

            // Reset energy cooldown and reduce energy when shield is active
            if (shield.isShieldActive)
            {
                currentEnergyRegenCooldown = energyRegenCooldown;
                float energyReduce = shieldCostPerSecond * Time.deltaTime;
                ModifyEnergy(-energyReduce);
            }

            // Reduce shield regen cooldown to zero
            if (currentEnergyRegenCooldown > 0 && !shield.isShieldActive)
            {
                currentEnergyRegenCooldown -= Time.deltaTime;
                if (currentEnergyRegenCooldown < 0)
                    currentEnergyRegenCooldown = 0;
            }

            // Regen shield
            if (energy < maxEnergy && currentEnergyRegenCooldown == 0 && !shield.isShieldActive)
            {
                float shieldRegen = shieldRegenPerSecond * Time.deltaTime;
                ModifyEnergy(shieldRegen);
            }
        }
    }

    public void OnHit(float damage)
    {
        ModifyHealth(-damage);
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

        GameEvents.RaiseOnHealthChanged(health);

        if (health <= 0 && !isDead)
        {
            isDead = true;
            GameEvents.RaiseOnPlayerDeath();
        }
    }

    // Method to add or remove shield
    public void ModifyEnergy(float changeEnergy)
    {
        energy += changeEnergy;

        // Restrict value between zero and max
        if (energy < 0)
            energy = 0;
        else if (energy > maxEnergy)
            energy = maxEnergy;

        GameEvents.RaiseOnEnergyChanged(energy);
    }
}
