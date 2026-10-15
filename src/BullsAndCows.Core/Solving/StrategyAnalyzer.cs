using BullsAndCows.Core.Model;

namespace BullsAndCows.Core.Solving;

/// <summary>Резултат от изиграване на стратегията срещу всяко възможно тайно число.</summary>
/// <param name="GuessCountDistribution">Ключ - брой опити, стойност - за колко числа са били нужни.</param>
public sealed record StrategyReport(IReadOnlyDictionary<int, int> GuessCountDistribution)
{
    public int TotalSecrets => GuessCountDistribution.Values.Sum();
    public int WorstCase => GuessCountDistribution.Keys.Max();
    public double Average => GuessCountDistribution.Sum(kv => (double)kv.Key * kv.Value) / TotalSecrets;
}

/// <summary>
/// Проверява стратегията изчерпателно срещу всички възможни тайни числа.
/// Вместо да играе 5040 отделни игри, строи дървото на решенията: тайните числа, които дават
/// еднакви отговори, минават по един и същ път, затова всеки възел се изчислява само веднъж.
/// Изборът на опит зависи единствено от списъка с кандидати (и реда им), така че резултатът
/// е идентичен с този от <see cref="CodeBreaker"/>.
/// </summary>
public static class StrategyAnalyzer
{
    public static StrategyReport Analyze(GameRules rules, IGuessStrategy strategy)
    {
        var allCodes = CodeSpace.Generate(rules);
        var distribution = new SortedDictionary<int, int>();
        Explore(allCodes, allCodes, strategy, attempt: 1, distribution);
        return new StrategyReport(distribution);
    }

    private static void Explore(IReadOnlyList<Code> candidates, IReadOnlyList<Code> allCodes,
                                IGuessStrategy strategy, int attempt, IDictionary<int, int> distribution)
    {
        Code guess = strategy.SelectGuess(candidates, allCodes);

        var partitions = new List<Code>?[Feedback.IndexCount];
        foreach (Code candidate in candidates)
        {
            int index = FeedbackCalculator.CalculateIndex(candidate, guess);
            (partitions[index] ??= new List<Code>()).Add(candidate);
        }

        for (int index = 0; index < partitions.Length; index++)
        {
            if (partitions[index] is not { } group)
                continue;

            if (index == Feedback.Win.Index)
                distribution[attempt] = distribution.TryGetValue(attempt, out int n) ? n + 1 : 1;
            else
                Explore(group, allCodes, strategy, attempt + 1, distribution);
        }
    }
}
