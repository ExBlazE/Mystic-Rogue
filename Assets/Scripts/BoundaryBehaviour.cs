using UnityEngine;

/// <summary>
/// Sets the bounds of the attached collider as play area.
/// If player goes out of play area, they take damage.
/// </summary>
public class BoundaryBehaviour : MonoBehaviour
{
    [SerializeField] float healthPenalty = 5f;

    [Space]
    [SerializeField] Player player;
    [SerializeField] ParticleSystem bleedFX;
    
    private bool isInPlayArea;

    void Start()
    {
        isInPlayArea = true;
        PlayBleedFX(false);
    }

    void Update()
    {
        // If player not in play area, reduce health
        if (!isInPlayArea)
        {
            float reduceHealth = healthPenalty * Time.deltaTime;
            player.ModifyHealth(-reduceHealth);
        }
    }

    // Detect when player leaves play area
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player.gameObject)
        {
            isInPlayArea = false;
            PlayBleedFX(true);
        }
    }

    // Detect when player enters play area
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player.gameObject)
        {
            isInPlayArea = true;
            PlayBleedFX(false);
        }
    }

    // Toggles bleeding FX on the player
    void PlayBleedFX(bool control)
    {
        var bleedEmission = bleedFX.emission;
        bleedEmission.enabled = control;
    }
}
