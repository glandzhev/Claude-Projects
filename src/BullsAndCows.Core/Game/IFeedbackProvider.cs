using BullsAndCows.Core.Model;

namespace BullsAndCows.Core.Game;

/// <summary>Източник на отговорите за всеки опит - човек, който мисли число, или автоматично изчисление.</summary>
public interface IFeedbackProvider
{
    Feedback GetFeedback(Code guess, int attempt);
}
