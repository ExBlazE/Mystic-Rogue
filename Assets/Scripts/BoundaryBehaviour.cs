using UnityEngine;

public class BoundaryBehaviour : MonoBehaviour
{
    [SerializeField] float healthPenalty = 5f;
    private bool isInPlayArea;

    void Start()
    {
        isInPlayArea = true;
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
            isInPlayArea = false;
    }

    // Detect when player enters play area
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isInPlayArea = true;
    }
}
