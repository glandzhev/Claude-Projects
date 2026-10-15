using BullsAndCows.Core.Game;
using BullsAndCows.Core.Model;

namespace BullsAndCows.ConsoleApp;

/// <summary>Показва опита на играча и чете от конзолата колко бика и крави има.</summary>
internal sealed class ConsoleFeedbackProvider : IFeedbackProvider
{
    public Feedback GetFeedback(Code guess, int attempt)
    {
        Console.WriteLine();
        Console.WriteLine($"Опит {attempt}: {guess}");

        while (true)
        {
            Console.Write("Бикове и крави (напр. \"1 2\"): ");
            string? line = Console.ReadLine() ?? throw new OperationCanceledException("Входът приключи.");

            if (FeedbackParser.TryParse(line, out var feedback, out var error))
                return feedback;

            Console.WriteLine($"  {error}");
        }
    }
}
