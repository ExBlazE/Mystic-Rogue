/// <summary>
/// Interface to be implemented by objects/targets that can be damaged (including player and enemies).
/// </summary>
public interface IDamageable
{
    public bool IsPlayerSide { get; }
    public Target TargetType { get; }
    public void OnHit(float damage);
}

/// <summary>
/// Enum to specify the type/faction of a target.
/// </summary>
public enum Target
{
    Neutral,
    Player,
    Shield,
    Enemy
}