using UnityEngine;

public class BoundaryBehaviour : MonoBehaviour
{
    [SerializeField] float healthPenalty = 5f;
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
            PlayerControl.Instance.ModifyHealth(-reduceHealth);
        }
    }

    // Detect when player leaves play area
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInPlayArea = false;
            PlayBleedFX(true);
        }
    }

    // Detect when player enters play area
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
