using BullsAndCows.Core.Game;
using BullsAndCows.Core.Model;
using BullsAndCows.Core.Solving;

namespace BullsAndCows.Tests;

public class SolverTests
{
    private readonly MinimaxGuessStrategy _strategy = new();

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Strategy_SolvesEverySecretWithinSevenAttempts(bool allowLeadingZero)
    {
        var rules = new GameRules(allowLeadingZero);

        var report = StrategyAnalyzer.Analyze(rules, _strategy);

        Assert.Equal(CodeSpace.Generate(rules).Count, report.TotalSecrets);
        Assert.True(report.WorstCase <= rules.MaxAttempts, $"Най-лош случай: {report.WorstCase}");
    }

    [Theory]
    [InlineData("0123")]
    [InlineData("9876")]
    [InlineData("5739")]
    [InlineData("1047")]
    [InlineData("8205")]
    public void GameRunner_SolvesSecret(string secret)
    {
        var code = Code.Parse(secret);
        var runner = new GameRunner(GameRules.Default, _strategy);

        var result = runner.Play(new SecretFeedbackProvider(code));

        Assert.Equal(GameOutcome.Solved, result.Outcome);
        Assert.Equal(code, result.Solution);
        Assert.InRange(result.Attempts, 1, GameRules.Default.MaxAttempts);
    }

    [Fact]
    public void GameRunner_NeverGuessesLeadingZero_WhenNotAllowed()
    {
        var rules = new GameRules(AllowLeadingZero: false);
        var runner = new GameRunner(rules, _strategy);

        var result = runner.Play(new SecretFeedbackProvider(Code.Parse("1098")));

        Assert.Equal(GameOutcome.Solved, result.Outcome);
        Assert.All(result.History, turn => Assert.False(turn.Guess.StartsWithZero));
    }

    [Fact]
    public void GameRunner_DetectsInconsistentFeedback()
    {
        // Първо "няма общи цифри", после "всички цифри са общи" за същите цифри - невъзможно.
        var provider = new ScriptedFeedbackProvider(new Feedback(0, 0), new Feedback(0, 4));
        var runner = new GameRunner(GameRules.Default, new FixedGuessStrategy(Code.Parse("0123"), Code.Parse("3210")));

        var result = runner.Play(provider);

        Assert.Equal(GameOutcome.InconsistentFeedback, result.Outcome);
        Assert.Null(result.Solution);
    }

    [Fact]
    public void CodeBreaker_KeepsOnlyConsistentCandidates()
    {
        var breaker = new CodeBreaker(GameRules.Default, _strategy);
        var secret = Code.Parse("4567");

        var guess = breaker.NextGuess();
        var feedback = FeedbackCalculator.Calculate(secret, guess);
        breaker.ApplyFeedback(feedback);

        Assert.Contains(secret, breaker.Candidates);
        Assert.All(breaker.Candidates, c => Assert.Equal(feedback, FeedbackCalculator.Calculate(c, guess)));
    }

    [Fact]
    public void CodeBreaker_RequiresGuessBeforeFeedback()
    {
        var breaker = new CodeBreaker(GameRules.Default, _strategy);

        Assert.Throws<InvalidOperationException>(() => breaker.ApplyFeedback(new Feedback(1, 1)));
    }

    private sealed class ScriptedFeedbackProvider(params Feedback[] answers) : IFeedbackProvider
    {
        public Feedback GetFeedback(Code guess, int attempt) => answers[attempt - 1];
    }

    private sealed class FixedGuessStrategy(params Code[] guesses) : IGuessStrategy
    {
        private int _next;

        public Code SelectGuess(IReadOnlyList<Code> candidates, IReadOnlyList<Code> allCodes) => guesses[_next++];
    }
}
