using System;
using UnityEngine;

/// <summary>
/// Contains all game-specific events and their public raise methods.
/// </summary>
public static class GameEvents
{
    public static event Action OnShotFired;
    public static event Action<Vector3, Quaternion, Target> OnShotHit;
    public static event Action OnShieldAppear;

    public static event Action<float> OnHealthChanged;
    public static event Action<float> OnEnergyChanged;
    public static event Action<int> OnScoreChanged;

    public static event Action OnPlayerDeath;
    public static event Action<int> OnEnemyDeath;

    public static event Action OnGameStart;
    public static event Action OnGamePause;
    public static event Action OnGameResume;
    public static event Action OnGameOver;
    public static event Action OnHighScore;

    // --- EVENT RAISE PUBLIC METHODS ---
    public static void RaiseOnShotFired() {  OnShotFired?.Invoke(); }
    public static void RaiseOnShotHit(Vector3 position, Quaternion rotation, Target targetType)
    { OnShotHit?.Invoke(position, rotation, targetType); }
    public static void RaiseOnShieldAppear() { OnShieldAppear?.Invoke(); }

    public static void RaiseOnHealthChanged(float newHealth) {  OnHealthChanged?.Invoke(newHealth); }
    public static void RaiseOnEnergyChanged(float newEnergy) {  OnEnergyChanged?.Invoke(newEnergy); }
    public static void RaiseOnScoreChanged(int newScore) { OnScoreChanged?.Invoke(newScore); }

    public static void RaiseOnPlayerDeath() { OnPlayerDeath?.Invoke(); }
    public static void RaiseOnEnemyDeath(int addScore) { OnEnemyDeath?.Invoke(addScore); }

    public static void RaiseOnGameStart() { OnGameStart?.Invoke(); }
    public static void RaiseOnGamePause() { OnGamePause?.Invoke(); }
    public static void RaiseOnGameResume() { OnGameResume?.Invoke(); }
    public static void RaiseOnGameOver() { OnGameOver?.Invoke(); }
    public static void RaiseOnHighScore() { OnHighScore?.Invoke(); }
}
