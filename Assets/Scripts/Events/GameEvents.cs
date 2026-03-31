using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action OnShotFired;
    public static event Action<Vector3, Quaternion, Target> OnShotHit;
    public static event Action OnShieldAppear;

    public static void ShotFired()
    {
        OnShotFired?.Invoke();
    }

    public static void ShotHit(Vector3 position, Quaternion rotation, Target targetType)
    {
        OnShotHit?.Invoke(position, rotation, targetType);
    }

    public static void ShieldAppear()
    {
        OnShieldAppear?.Invoke();
    }
}
