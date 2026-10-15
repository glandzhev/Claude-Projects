using System.Numerics;
using System.Runtime.CompilerServices;

namespace BullsAndCows.Core.Model;

public static class FeedbackCalculator
{
    public static Feedback Calculate(Code secret, Code guess)
    {
        int index = CalculateIndex(secret, guess);
        return new Feedback(index / (Code.Length + 1), index % (Code.Length + 1));
    }

    /// <summary>
    /// Бърз вариант без алокации, който връща директно <see cref="Feedback.Index"/>.
    /// Общите цифри са popcount на сечението на маските, а биковете са съвпаденията по позиция.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CalculateIndex(Code secret, Code guess)
    {
        int common = BitOperations.PopCount((uint)(secret.DigitMask & guess.DigitMask));
        int bulls = 0;
        for (int i = 0; i < Code.Length; i++)
        {
            if (secret[i] == guess[i])
                bulls++;
        }

        return bulls * (Code.Length + 1) + (common - bulls);
    }
}
