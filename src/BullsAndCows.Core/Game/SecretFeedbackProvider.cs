using BullsAndCows.Core.Model;

namespace BullsAndCows.Core.Game;

/// <summary>Изчислява отговорите автоматично по известно тайно число. Отгатващият не го вижда.</summary>
public sealed class SecretFeedbackProvider(Code secret) : IFeedbackProvider
{
    public Feedback GetFeedback(Code guess, int attempt) => FeedbackCalculator.Calculate(secret, guess);
}
