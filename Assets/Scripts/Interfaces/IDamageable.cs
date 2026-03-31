public interface IDamageable
{
    public bool IsPlayerSide { get; }
    public Target TargetType { get; }
    public void OnHit(float damage);
}

public enum Target
{
    Neutral,
    Player,
    Shield,
    Enemy
};