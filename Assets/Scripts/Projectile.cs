using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] bool isPlayerSide;

    [Space]
    [SerializeField] float speed = 15f;
    [SerializeField] float maxDuration = 3f;
    private float aliveDuration;

    [Space]
    [SerializeField] float damage = 10;

    private bool hasCollided = false;

    void OnEnable()
    {
        aliveDuration = maxDuration;
        hasCollided = false;
    }

    void Update()
    {
        // Move projectile forward at constant speed
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Destroy projectile after set duration
        if (aliveDuration > 0)
        {
            aliveDuration -= Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // This block is to prevent double collision cases
        if (hasCollided)
            return;

        if (other.TryGetComponent<IDamageable>(out var target))
        {
            bool isFriendlyFire = (this.isPlayerSide == target.IsPlayerSide);
            if (!isFriendlyFire)
            {
                hasCollided = true;
                target.OnHit(damage);
                GameEvents.ShotHit(transform.position, transform.rotation, target.TargetType);
                gameObject.SetActive(false);
            }
        }
    }
}
