namespace Fundo.LoanEngine.Domain.ValueObjects;

public sealed record Ssn
{
    private Ssn(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public string Formatted => $"{Value[..3]}-{Value[3..5]}-{Value[5..]}";

    public static Ssn Create(string? value)
    {
        if (!TryCreate(value, out var ssn))
        {
            throw new ArgumentException("SSN must contain exactly nine digits.", nameof(value));
        }

        return ssn;
    }

    public static bool TryCreate(string? value, out Ssn ssn)
    {
        var digits = new string((value ?? string.Empty)
            .Where(character => char.IsAsciiDigit(character))
            .ToArray());
        var containsOnlyAllowedCharacters = (value ?? string.Empty)
            .All(character => char.IsAsciiDigit(character) || character == '-' || char.IsWhiteSpace(character));

        if (!containsOnlyAllowedCharacters || digits.Length != 9)
        {
            ssn = null!;
            return false;
        }

        ssn = new Ssn(digits);
        return true;
    }
}