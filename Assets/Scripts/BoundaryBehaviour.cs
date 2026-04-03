using UnityEngine;

public class BoundaryBehaviour : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] ParticleSystem bleedFX;
    [SerializeField] float healthPenalty = 5f;
    
    private bool isInPlayArea;

    void Start()
    {
        isInPlayArea = true;
        PlayBleedFX(false);
    }

    void Update()
    {
        // If player not in play area, reduce health
        if (GameManager.Instance.gameState == GameState.Playing && !isInPlayArea)
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

    void PlayBleedFX(bool control)
    {
        var bleedEmission = bleedFX.emission;
        bleedEmission.enabled = control;
    }
}
