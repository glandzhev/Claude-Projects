namespace BullsAndCows.Core.Model;

/// <summary>Отговор на опит: брой бикове (вярна цифра на вярно място) и крави (вярна цифра на грешно място).</summary>
public readonly record struct Feedback
{
    /// <summary>Броят на всички различни индекси, които <see cref="Index"/> може да върне.</summary>
    public const int IndexCount = (Code.Length + 1) * (Code.Length + 1);

    public Feedback(int bulls, int cows)
    {
        if (bulls < 0 || cows < 0 || bulls + cows > Code.Length)
            throw new ArgumentException($"Биковете и кравите трябва да са неотрицателни и общо най-много {Code.Length}.");
        // При 3 бика последната обща цифра няма къде другаде да отиде, затова 3 бика + 1 крава е невъзможно.
        if (bulls == Code.Length - 1 && cows == 1)
            throw new ArgumentException($"{Code.Length - 1} бика и 1 крава е невъзможна комбинация.");

        Bulls = bulls;
        Cows = cows;
    }

    public int Bulls { get; }
    public int Cows { get; }

    public bool IsWin => Bulls == Code.Length;

    /// <summary>Компактен индекс, удобен за групиране на кандидатите в масив.</summary>
    public int Index => Bulls * (Code.Length + 1) + Cows;

    public static Feedback Win { get; } = new(Code.Length, 0);

    public override string ToString() =>
        $"{Bulls} {(Bulls == 1 ? "бик" : "бика")}, {Cows} {(Cows == 1 ? "крава" : "крави")}";
}
