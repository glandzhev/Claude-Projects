using System.Diagnostics;
using System.Text;
using BullsAndCows.ConsoleApp;
using BullsAndCows.Core.Game;
using BullsAndCows.Core.Model;
using BullsAndCows.Core.Solving;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var rules = new GameRules(AllowLeadingZero: !args.Contains("--no-leading-zero"));
var strategy = new MinimaxGuessStrategy();

if (args.Contains("--analyze"))
{
    RunAnalysis(rules, strategy);
    return;
}

Console.WriteLine("=== Бикове и крави ===");
Console.WriteLine($"Измисли число от 4 различни цифри{(rules.AllowLeadingZero ? "" : " (без водеща нула)")}.");
Console.WriteLine($"Ще го позная до {rules.MaxAttempts} опита.");
Console.WriteLine("Бик = вярна цифра на вярно място, крава = вярна цифра на грешно място.");

try
{
    do
    {
        PlayOnce(rules, strategy);
    } while (AskYesNo("\nОще една игра? (д/н): "));
}
catch (OperationCanceledException)
{
    // Входът е затворен (Ctrl+Z / Ctrl+D) - просто излизаме.
    Console.WriteLine();
}

static void PlayOnce(GameRules rules, IGuessStrategy strategy)
{
    Console.WriteLine();
    Console.WriteLine("1) Държа числото наум и сам отговарям с бикове и крави");
    Console.WriteLine("2) Въвеждам числото и отговорите се смятат автоматично");
    IFeedbackProvider provider = ReadChoice() == 1
        ? new ConsoleFeedbackProvider()
        : new SecretFeedbackProvider(ReadSecret(rules));

    bool isManual = provider is ConsoleFeedbackProvider;
    var runner = new GameRunner(rules, strategy);
    runner.TurnCompleted += (attempt, guess, feedback, remaining) =>
    {
        string prefix = isManual ? "  ->" : $"Опит {attempt}: {guess} ->";
        Console.WriteLine($"{prefix} {feedback}, възможни числа: {remaining}");
    };

    GameResult result = runner.Play(provider);

    Console.WriteLine();
    Console.WriteLine(result.Outcome switch
    {
        GameOutcome.Solved => $"Познах! Числото е {result.Solution} - от {result.Attempts} {(result.Attempts == 1 ? "опит" : "опита")}.",
        GameOutcome.InconsistentFeedback =>
            "Няма число, което да отговаря на всички дадени отговори. Вероятно някъде има грешка при броенето.",
        _ => $"Не успях за {rules.MaxAttempts} опита. Провери отговорите си.",
    });
}

static int ReadChoice()
{
    while (true)
    {
        Console.Write("Избор (1/2): ");
        string? line = Console.ReadLine()?.Trim() ?? throw new OperationCanceledException("Входът приключи.");
        if (line is "1" or "2")
            return int.Parse(line);
    }
}

static Code ReadSecret(GameRules rules)
{
    while (true)
    {
        Console.Write("Твоето число: ");
        string? line = Console.ReadLine() ?? throw new OperationCanceledException("Входът приключи.");

        if (!Code.TryParse(line, out var code, out var error))
            Console.WriteLine($"  {error}");
        else if (!rules.AllowLeadingZero && code.StartsWithZero)
            Console.WriteLine("  Числото не може да започва с 0.");
        else
            return code;
    }
}

static bool AskYesNo(string question)
{
    Console.Write(question);
    string answer = Console.ReadLine()?.Trim().ToLowerInvariant() ?? string.Empty;
    return answer is "д" or "да" or "y" or "yes";
}

static void RunAnalysis(GameRules rules, IGuessStrategy strategy)
{
    Console.WriteLine("Изиграване на стратегията срещу всички възможни числа...");
    var stopwatch = Stopwatch.StartNew();
    StrategyReport report = StrategyAnalyzer.Analyze(rules, strategy);
    stopwatch.Stop();

    Console.WriteLine($"Числа: {report.TotalSecrets}, време: {stopwatch.Elapsed.TotalSeconds:F1} с");
    foreach (var (guesses, count) in report.GuessCountDistribution)
        Console.WriteLine($"  {guesses} {(guesses == 1 ? "опит" : "опита")}: {count}");
    Console.WriteLine($"Среден брой опити: {report.Average:F3}");
    Console.WriteLine($"Най-лош случай: {report.WorstCase} (лимит: {rules.MaxAttempts})");
}
