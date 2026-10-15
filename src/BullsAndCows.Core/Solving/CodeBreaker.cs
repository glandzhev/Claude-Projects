using BullsAndCows.Core.Model;

namespace BullsAndCows.Core.Solving;

/// <summary>
/// Пази текущото състояние на отгатването: оставащите кандидати и историята на опитите.
/// Никога не вижда тайното число - работи само с отговорите.
/// </summary>
public sealed class CodeBreaker
{
    private readonly IGuessStrategy _strategy;
    private readonly IReadOnlyList<Code> _allCodes;
    private readonly List<(Code Guess, Feedback Feedback)> _history = new();
    private List<Code> _candidates;
    private Code? _pendingGuess;

    public CodeBreaker(GameRules rules, IGuessStrategy strategy)
    {
        _strategy = strategy;
        _allCodes = CodeSpace.Generate(rules);
        _candidates = _allCodes.ToList();
    }

    public IReadOnlyList<Code> Candidates => _candidates;
    public IReadOnlyList<(Code Guess, Feedback Feedback)> History => _history;

    /// <summary>True, ако отговорите си противоречат и няма число, което да ги удовлетворява.</summary>
    public bool HasContradiction => _candidates.Count == 0;

    public Code NextGuess()
    {
        if (HasContradiction)
            throw new InvalidOperationException("Отговорите си противоречат - няма възможно число.");

        _pendingGuess ??= _strategy.SelectGuess(_candidates, _allCodes);
        return _pendingGuess.Value;
    }

    public void ApplyFeedback(Feedback feedback)
    {
        if (_pendingGuess is not { } guess)
            throw new InvalidOperationException($"Извикай {nameof(NextGuess)} преди {nameof(ApplyFeedback)}.");

        _history.Add((guess, feedback));
        _candidates = _candidates
            .Where(c => FeedbackCalculator.CalculateIndex(c, guess) == feedback.Index)
            .ToList();
        _pendingGuess = null;
    }
}
