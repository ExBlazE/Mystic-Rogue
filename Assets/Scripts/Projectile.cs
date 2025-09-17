using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 15f;
    [SerializeField] float duration = 3f;

    [Space]
    [SerializeField] float enemyShotDamage = 10;

    [Space]
    [SerializeField] ParticleSystem playerHitFX;
    [SerializeField] ParticleSystem shieldHitFX;
    [SerializeField] ParticleSystem enemyHitFX;

    void Update()
    {
        // Move projectile forward at constant speed
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Destroy projectile after set duration
        if (duration > 0)
        {
            duration -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Logic for player projectiles hitting enemies
        if (gameObject.CompareTag("Shot_Player") && other.CompareTag("Enemy"))
        {
            Transform particlesGroup = GameManager.Instance.particlesGroupObject;
            Instantiate(playerHitFX, transform.position, transform.rotation, particlesGroup);
            GameManager.Instance.AddScore(1);

            Destroy(other.gameObject);
            Destroy(gameObject);

            GameManager.Instance.enemiesOnScreen--;
            AudioManager.Instance.PlayShotHit();
        }

        // Logic for enemy projectiles
        else if (gameObject.CompareTag("Shot_Enemy"))
        {
            // Logic for hitting shield
            if (other.CompareTag("Shield"))
            {
                Transform particlesGroup = GameManager.Instance.particlesGroupObject;
                Instantiate(shieldHitFX, transform.position, transform.rotation, particlesGroup);

                PlayerControl.Instance.ModifyShield(-enemyShotDamage);
                Destroy(gameObject);
                
                AudioManager.Instance.PlayShotHit();
            }

            //Logic for hitting player
            else if (other.CompareTag("Player"))
            {
                Transform particlesGroup = GameManager.Instance.particlesGroupObject;
                Instantiate(enemyHitFX, transform.position, transform.rotation, particlesGroup);

                PlayerControl.Instance.ModifyHealth(-enemyShotDamage);
                Destroy(gameObject);
                
                AudioManager.Instance.PlayShotHit();
            }
        }
    }
}
