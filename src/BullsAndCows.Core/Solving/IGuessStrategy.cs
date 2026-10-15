using BullsAndCows.Core.Model;

namespace BullsAndCows.Core.Solving;

public interface IGuessStrategy
{
    /// <summary>Избира следващия опит.</summary>
    /// <param name="candidates">Числата, които още са съвместими с всички получени отговори. Никога не е празен.</param>
    /// <param name="allCodes">Всички допустими числа - опитът не е задължително да е кандидат.</param>
    Code SelectGuess(IReadOnlyList<Code> candidates, IReadOnlyList<Code> allCodes);
}
