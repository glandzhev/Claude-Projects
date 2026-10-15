namespace BullsAndCows.Core.Model;

/// <param name="AllowLeadingZero">Дали тайното число може да започва с 0.</param>
/// <param name="MaxAttempts">Максимален брой опити, включително печелившия.</param>
public sealed record GameRules(bool AllowLeadingZero = true, int MaxAttempts = 7)
{
    public static GameRules Default { get; } = new();
}
