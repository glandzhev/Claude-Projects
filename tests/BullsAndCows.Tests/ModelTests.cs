using BullsAndCows.Core.Model;

namespace BullsAndCows.Tests;

public class ModelTests
{
    [Theory]
    [InlineData("1234", "1234", 4, 0)]
    [InlineData("1234", "4321", 0, 4)]
    [InlineData("1234", "5678", 0, 0)]
    [InlineData("1234", "1243", 2, 2)]
    [InlineData("1234", "1567", 1, 0)]
    [InlineData("0123", "3210", 0, 4)]
    [InlineData("5739", "3759", 2, 2)]
    [InlineData("5739", "3951", 0, 3)]
    public void Calculate_ReturnsBullsAndCows(string secret, string guess, int bulls, int cows)
    {
        var feedback = FeedbackCalculator.Calculate(Code.Parse(secret), Code.Parse(guess));

        Assert.Equal(new Feedback(bulls, cows), feedback);
    }

    [Fact]
    public void Calculate_IsSymmetric()
    {
        var codes = CodeSpace.Generate(GameRules.Default);
        var random = new Random(42);

        for (int i = 0; i < 1000; i++)
        {
            var a = codes[random.Next(codes.Count)];
            var b = codes[random.Next(codes.Count)];
            Assert.Equal(FeedbackCalculator.Calculate(a, b), FeedbackCalculator.Calculate(b, a));
        }
    }

    [Theory]
    [InlineData("123")]
    [InlineData("12345")]
    [InlineData("1123")]
    [InlineData("12a4")]
    [InlineData("")]
    [InlineData(null)]
    public void TryParse_RejectsInvalidCodes(string? text)
    {
        Assert.False(Code.TryParse(text, out _, out var error));
        Assert.NotNull(error);
    }

    [Theory]
    [InlineData("0123", "0123")]
    [InlineData(" 9 8 7 6 ", "9876")]
    public void TryParse_AcceptsValidCodes(string text, string expected)
    {
        Assert.True(Code.TryParse(text, out var code, out _));
        Assert.Equal(expected, code.ToString());
    }

    [Theory]
    [InlineData(3, 1)]
    [InlineData(2, 3)]
    [InlineData(-1, 0)]
    [InlineData(5, 0)]
    public void Feedback_RejectsImpossibleCombinations(int bulls, int cows)
    {
        Assert.Throws<ArgumentException>(() => new Feedback(bulls, cows));
    }

    [Theory]
    [InlineData(true, 5040)]
    [InlineData(false, 4536)]
    public void CodeSpace_HasExpectedSize(bool allowLeadingZero, int expected)
    {
        var codes = CodeSpace.Generate(new GameRules(allowLeadingZero));

        Assert.Equal(expected, codes.Count);
        Assert.Equal(expected, codes.Distinct().Count());
    }

    [Theory]
    [InlineData("1 2", 1, 2)]
    [InlineData("0,0", 0, 0)]
    [InlineData("13", 1, 3)]
    [InlineData("  4 0 ", 4, 0)]
    public void FeedbackParser_AcceptsCommonFormats(string input, int bulls, int cows)
    {
        Assert.True(FeedbackParser.TryParse(input, out var feedback, out _));
        Assert.Equal(new Feedback(bulls, cows), feedback);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1")]
    [InlineData("a b")]
    [InlineData("3 1")]
    [InlineData("1 2 3")]
    public void FeedbackParser_RejectsInvalidInput(string input)
    {
        Assert.False(FeedbackParser.TryParse(input, out _, out var error));
        Assert.NotNull(error);
    }
}
