using System.Numerics;

namespace BullsAndCows.Core.Model;

/// <summary>
/// Четирицифрено число с неповтарящи се цифри. Immutable value object.
/// </summary>
public readonly struct Code : IEquatable<Code>
{
    public const int Length = 4;

    private readonly int _packed; // цифра i е в битове [4*i, 4*i+3]

    private Code(int packed, int mask)
    {
        _packed = packed;
        DigitMask = mask;
    }

    /// <summary>Битова маска на използваните цифри (бит d е вдигнат, ако цифрата d участва).</summary>
    public int DigitMask { get; }

    public int this[int position] => (_packed >> (4 * position)) & 0xF;

    public bool StartsWithZero => this[0] == 0;

    public static Code FromDigits(ReadOnlySpan<int> digits)
    {
        if (digits.Length != Length)
            throw new ArgumentException($"Числото трябва да е с точно {Length} цифри.", nameof(digits));

        int packed = 0, mask = 0;
        for (int i = 0; i < Length; i++)
        {
            int d = digits[i];
            if (d is < 0 or > 9)
                throw new ArgumentOutOfRangeException(nameof(digits), "Допустими са само цифри от 0 до 9.");
            if ((mask & (1 << d)) != 0)
                throw new ArgumentException("Цифрите не трябва да се повтарят.", nameof(digits));

            mask |= 1 << d;
            packed |= d << (4 * i);
        }

        return new Code(packed, mask);
    }

    public static bool TryParse(string? text, out Code code, out string? error)
    {
        code = default;
        string digitsOnly = new((text ?? string.Empty).Where(c => !char.IsWhiteSpace(c)).ToArray());

        if (digitsOnly.Length != Length || !digitsOnly.All(char.IsAsciiDigit))
        {
            error = $"Въведи точно {Length} цифри.";
            return false;
        }

        Span<int> digits = stackalloc int[Length];
        for (int i = 0; i < Length; i++)
            digits[i] = digitsOnly[i] - '0';

        int mask = 0;
        foreach (int d in digits)
            mask |= 1 << d;
        if (BitOperations.PopCount((uint)mask) != Length)
        {
            error = "Цифрите не трябва да се повтарят.";
            return false;
        }

        code = FromDigits(digits);
        error = null;
        return true;
    }

    public static Code Parse(string text) =>
        TryParse(text, out var code, out var error) ? code : throw new FormatException(error);

    public bool Equals(Code other) => _packed == other._packed;
    public override bool Equals(object? obj) => obj is Code other && Equals(other);
    public override int GetHashCode() => _packed;
    public static bool operator ==(Code left, Code right) => left.Equals(right);
    public static bool operator !=(Code left, Code right) => !left.Equals(right);

    public override string ToString() =>
        string.Create(Length, this, static (span, code) =>
        {
            for (int i = 0; i < Length; i++)
                span[i] = (char)('0' + code[i]);
        });
}
