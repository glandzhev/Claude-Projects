using BullsAndCows.Core.Model;
using BullsAndCows.Core.Solving;

namespace BullsAndCows.Core.Game;

public sealed class GameRunner(GameRules rules, IGuessStrategy strategy)
{
    /// <summary>Вика се след всеки опит с номера му, опита, отговора и броя оставащи кандидати.</summary>
    public event Action<int, Code, Feedback, int>? TurnCompleted;

    public GameResult Play(IFeedbackProvider feedbackProvider)
    {
        var breaker = new CodeBreaker(rules, strategy);

        for (int attempt = 1; attempt <= rules.MaxAttempts; attempt++)
        {
            if (breaker.HasContradiction)
                return new GameResult(GameOutcome.InconsistentFeedback, breaker.History, null);

            Code guess = breaker.NextGuess();
            Feedback feedback = feedbackProvider.GetFeedback(guess, attempt);
            breaker.ApplyFeedback(feedback);
            TurnCompleted?.Invoke(attempt, guess, feedback, breaker.Candidates.Count);

            if (feedback.IsWin)
                return new GameResult(GameOutcome.Solved, breaker.History, guess);
        }

        return new GameResult(
            breaker.HasContradiction ? GameOutcome.InconsistentFeedback : GameOutcome.OutOfAttempts,
            breaker.History,
            null);
    }
}
