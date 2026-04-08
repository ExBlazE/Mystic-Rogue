using System;
using UnityEngine;

/// <summary>
/// Contains all input-specific events and their public raise methods.
/// </summary>
public static class InputEvents
{
    public static event Action<Vector2> OnMove;
    public static event Action<Vector3> OnAim;
    public static event Action OnShoot;
    public static event Action<bool> OnShield;

    public static event Action OnPauseRequest;

    // --- EVENT RAISE PUBLIC METHODS ---
    public static void RaiseOnMove(Vector2 moveDirection) { OnMove?.Invoke(moveDirection); }
    public static void RaiseOnAim(Vector3 aimDirection) { OnAim?.Invoke(aimDirection); }
    public static void RaiseOnShoot() { OnShoot?.Invoke(); }
    public static void RaiseOnShield(bool isShieldStart) { OnShield?.Invoke(isShieldStart); }

    public static void RaiseOnPauseRequest() { OnPauseRequest?.Invoke(); }
}
