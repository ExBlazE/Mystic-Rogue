using UnityEngine;

/// <summary>
/// Manages the particle effects appearing when projectiles hit a valid target.
/// </summary>
public class ProjectileFXHandler : MonoBehaviour
{
    [SerializeField] ObjectPooler enemyVFXPool;
    [SerializeField] ObjectPooler playerVFXPool;
    [SerializeField] ObjectPooler shieldVFXPool;

    void OnEnable()
    {
        GameEvents.OnShotHit += PlayShotHitFX;
    }

    void OnDisable()
    {
        GameEvents.OnShotHit -= PlayShotHitFX;
    }

    void PlayShotHitFX(Vector3 position, Quaternion rotation, Target targetType)
    {
        switch (targetType)
        {
            case Target.Enemy:
                enemyVFXPool.GetFromPool(position, rotation);
                break;

            case Target.Player:
                playerVFXPool.GetFromPool(position, rotation);
                break;

            case Target.Shield:
                shieldVFXPool.GetFromPool(position, rotation);
                break;

            default:
                break;
        }
    }
}
