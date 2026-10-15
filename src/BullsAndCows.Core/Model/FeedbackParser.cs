namespace BullsAndCows.Core.Model;

public static class FeedbackParser
{
    private static readonly char[] Separators = [' ', ',', ';', '/', '-', '\t'];

    /// <summary>Приема "1 2", "1,2" или "12" (бикове, после крави).</summary>
    public static bool TryParse(string? input, out Feedback feedback, out string? error)
    {
        feedback = default;
        string text = (input ?? string.Empty).Trim();

        string[] parts = text.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1 && parts[0].Length == 2)
            parts = [parts[0][..1], parts[0][1..]];

        if (parts.Length != 2 || !int.TryParse(parts[0], out int bulls) || !int.TryParse(parts[1], out int cows))
        {
            error = "Въведи две числа - бикове и крави, напр. \"1 2\".";
            return false;
        }

        try
        {
            feedback = new Feedback(bulls, cows);
            error = null;
            return true;
        }
        catch (ArgumentException ex)
        {
            error = ex.Message;
            return false;
        }
    }
}
