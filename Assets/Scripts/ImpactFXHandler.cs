using UnityEngine;

public class ProjectileFXHandler : MonoBehaviour
{
    public ObjectPooler enemyVFXPool;
    public ObjectPooler playerVFXPool;
    public ObjectPooler shieldVFXPool;

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
        GameObject particleFX;

        switch (targetType)
        {
            case Target.Enemy:
                particleFX = enemyVFXPool.GetFromPool();
                break;

            case Target.Player:
                particleFX = playerVFXPool.GetFromPool();
                break;

            case Target.Shield:
                particleFX = shieldVFXPool.GetFromPool();
                break;

            default:
                particleFX = null;
                break;
        }

        particleFX.transform.position = position;
        particleFX.transform.rotation = rotation;
    }
}
