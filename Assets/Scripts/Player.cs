using UnityEngine;

/// <summary>
/// Manages player stats and their modification methods.<br/>
/// <i>MANDATORY for every scene with controllable player.</i>
/// </summary>
public class Player : MonoBehaviour, IDamageable
{
    public bool IsPlayerSide => true;
    public Target TargetType => Target.Player;

    public float MaxHealth = 100f;
    public float MaxEnergy = 50f;

    [Header("Shield")]
    [SerializeField] float shieldRegenPerSecond = 10f;
    [SerializeField] float shieldCostPerSecond = 5f;
    [SerializeField] float energyRegenCooldown = 3f;
    [SerializeField] Shield shield;

    public float Health { get; private set; }
    public float Energy { get; private set; }

    private float currentEnergyRegenCooldown;
    private bool isDead;

    void Awake()
    {
        // Set starting health and shield to max
        Health = MaxHealth;
        Energy = MaxEnergy;
        isDead = false;
    }

    void Start()
    {
        // Reset cooldowns
        currentEnergyRegenCooldown = 0;
    }

    void Update()
    {
        // Energy and shield logic below to run only when game is active and unpaused
        if (GameManager.Instance.gameState != GameState.Playing) return;

        // Disable shield when zero
        if (Energy == 0 && shield.IsShieldActive)
        { shield.Toggle(false); }

        // Reset energy cooldown and reduce energy when shield is active
        if (shield.IsShieldActive)
        {
            currentEnergyRegenCooldown = energyRegenCooldown;
            float energyReduce = shieldCostPerSecond * Time.deltaTime;
            ModifyEnergy(-energyReduce);
        }

        // Reduce shield regen cooldown to zero
        if (currentEnergyRegenCooldown > 0 && !shield.IsShieldActive)
        {
            currentEnergyRegenCooldown -= Time.deltaTime;
            if (currentEnergyRegenCooldown < 0)
                currentEnergyRegenCooldown = 0;
        }

        // Regen shield
        if (Energy < MaxEnergy && currentEnergyRegenCooldown == 0 && !shield.IsShieldActive)
        {
            float shieldRegen = shieldRegenPerSecond * Time.deltaTime;
            ModifyEnergy(shieldRegen);
        }
    }

    public void OnHit(float damage)
    {
        ModifyHealth(-damage);
    }

    // Method to add or remove health
    public void ModifyHealth(float changeHealth)
    {
        Health += changeHealth;

        // Restrict value between zero and max
        if (Health < 0)
            Health = 0;
        else if (Health > MaxHealth)
            Health = MaxHealth;

        GameEvents.RaiseOnHealthChanged(Health);

        // If health becomes zero, set player to dead
        if (Health <= 0 && !isDead)
        {
            isDead = true;
            GameEvents.RaiseOnPlayerDeath();
        }
    }

    // Method to add or remove shield
    public void ModifyEnergy(float changeEnergy)
    {
        Energy += changeEnergy;

        // Restrict value between zero and max
        if (Energy < 0)
            Energy = 0;
        else if (Energy > MaxEnergy)
            Energy = MaxEnergy;

        GameEvents.RaiseOnEnergyChanged(Energy);
    }
}
