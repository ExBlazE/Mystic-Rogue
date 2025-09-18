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

    private bool hasCollided = false;

    GameManager gm;
    AudioManager am;
    PlayerControl player;

    void Start()
    {
        gm = GameManager.Instance;
        am = AudioManager.Instance;
        player = PlayerControl.Instance;
    }

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
        // This block is to prevent double collision cases
        if (hasCollided)
            return;

        // Logic for player projectiles hitting enemies
        if (gameObject.CompareTag("Shot_Player") && other.CompareTag("Enemy"))
        {
            hasCollided = true;

            Transform particlesGroup = gm.particlesGroup;
            Instantiate(playerHitFX, transform.position, transform.rotation, particlesGroup);
            gm.AddScore(1);

            other.gameObject.SetActive(false);
            Destroy(gameObject);

            gm.enemiesOnScreen--;
            am.PlayShotHit();
        }

        // Logic for enemy projectiles
        else if (gameObject.CompareTag("Shot_Enemy"))
        {
            // Logic for hitting shield
            if (other.CompareTag("Shield"))
            {
                hasCollided = true;

                Transform particlesGroup = gm.particlesGroup;
                Instantiate(shieldHitFX, transform.position, transform.rotation, particlesGroup);

                player.ModifyShield(-enemyShotDamage);
                Destroy(gameObject);
                
                am.PlayShotHit();
            }

            //Logic for hitting player
            else if (other.CompareTag("Player"))
            {
                hasCollided = true;

                Transform particlesGroup = gm.particlesGroup;
                Instantiate(enemyHitFX, transform.position, transform.rotation, particlesGroup);

                player.ModifyHealth(-enemyShotDamage);
                Destroy(gameObject);
                
                am.PlayShotHit();
            }
        }
    }
}
