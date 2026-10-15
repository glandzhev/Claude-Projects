using BullsAndCows.Core.Model;

namespace BullsAndCows.Core.Game;

public enum GameOutcome
{
    Solved,
    OutOfAttempts,
    /// <summary>Отговорите си противоречат - вероятно е допусната грешка при броенето.</summary>
    InconsistentFeedback,
}

public sealed record GameResult(
    GameOutcome Outcome,
    IReadOnlyList<(Code Guess, Feedback Feedback)> History,
    Code? Solution)
{
    public int Attempts => History.Count;
}
