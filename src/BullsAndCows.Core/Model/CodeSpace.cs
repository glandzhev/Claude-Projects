namespace BullsAndCows.Core.Model;

public static class CodeSpace
{
    /// <summary>
    /// Всички допустими числа в лексикографски ред: 10*9*8*7 = 5040 с водеща нула или 9*9*8*7 = 4536 без.
    /// </summary>
    public static IReadOnlyList<Code> Generate(GameRules rules)
    {
        var result = new List<Code>(5040);
        Span<int> digits = stackalloc int[Code.Length];
        Fill(digits, 0, 0, rules.AllowLeadingZero, result);
        return result;
    }

    private static void Fill(Span<int> digits, int position, int usedMask, bool allowLeadingZero, List<Code> result)
    {
        if (position == Code.Length)
        {
            result.Add(Code.FromDigits(digits));
            return;
        }

        int first = position == 0 && !allowLeadingZero ? 1 : 0;
        for (int d = first; d <= 9; d++)
        {
            if ((usedMask & (1 << d)) != 0)
                continue;

            digits[position] = d;
            Fill(digits, position + 1, usedMask | (1 << d), allowLeadingZero, result);
        }
    }
}
