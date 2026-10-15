using BullsAndCows.Core.Model;

namespace BullsAndCows.Core.Solving;

/// <summary>
/// Стратегия на Кнут (minimax), приложена към Бикове и крави.
///
/// Всеки възможен опит разделя оставащите кандидати на групи според отговора, който би се получил.
/// Избираме опита, при който най-голямата група е най-малка - така гарантираме най-доброто
/// поведение в най-лошия случай. При равенство:
///   1) по-малка сума от квадратите на групите (= по-малък очакван брой оставащи кандидати);
///   2) опит, който сам е кандидат (има шанс да спечелим веднага);
///   3) първият по ред, за да е алгоритъмът детерминиран.
/// </summary>
public sealed class MinimaxGuessStrategy : IGuessStrategy
{
    public Code SelectGuess(IReadOnlyList<Code> candidates, IReadOnlyList<Code> allCodes)
    {
        if (candidates.Count == 0)
            throw new ArgumentException("Няма кандидати.", nameof(candidates));

        // С 1 или 2 кандидата няма как да се справим по-добре от това да пробваме първия.
        if (candidates.Count <= 2)
            return candidates[0];

        var candidateSet = new HashSet<Code>(candidates);
        Span<int> partitionSizes = stackalloc int[Feedback.IndexCount];

        Code best = candidates[0];
        var bestScore = (Worst: int.MaxValue, SumOfSquares: long.MaxValue, IsCandidate: false);

        foreach (Code guess in allCodes)
        {
            partitionSizes.Clear();
            foreach (Code candidate in candidates)
                partitionSizes[FeedbackCalculator.CalculateIndex(candidate, guess)]++;

            int worst = 0;
            long sumOfSquares = 0;
            foreach (int size in partitionSizes)
            {
                worst = Math.Max(worst, size);
                sumOfSquares += (long)size * size;
            }

            // Ранно отхвърляне - голяма част от опитите се отрязват още тук.
            if (worst > bestScore.Worst)
                continue;

            var score = (worst, sumOfSquares, candidateSet.Contains(guess));
            if (IsBetter(score, bestScore))
            {
                best = guess;
                bestScore = score;
            }
        }

        return best;
    }

    private static bool IsBetter((int Worst, long SumOfSquares, bool IsCandidate) a,
                                 (int Worst, long SumOfSquares, bool IsCandidate) b)
    {
        if (a.Worst != b.Worst) return a.Worst < b.Worst;
        if (a.SumOfSquares != b.SumOfSquares) return a.SumOfSquares < b.SumOfSquares;
        return a.IsCandidate && !b.IsCandidate;
    }
}
