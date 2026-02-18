using System.Text.RegularExpressions;

namespace BauDoku.BuildingBlocks.Domain.Guards;

public readonly ref struct StringGuard
{
    private readonly string? value;
    private readonly string paramName;

    internal StringGuard(string? value, string paramName)
    {
        this.value = value;
        this.paramName = paramName;
    }

    public StringGuard IsNotNullOrWhiteSpace(string? message = null)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(message ?? $"{paramName} darf nicht leer sein.", paramName);
        return this;
    }

    public StringGuard MaxLengthIs(int maxLength, string? message = null)
    {
        if (value is not null && value.Length > maxLength) throw new ArgumentException(message ?? $"{paramName} darf max. {maxLength} Zeichen lang sein.", paramName);
        return this;
    }

    public StringGuard MinLengthIs(int minLength, string? message = null)
    {
        if (value is not null && value.Length < minLength) throw new ArgumentException(message ?? $"{paramName} muss mind. {minLength} Zeichen lang sein.", paramName);
        return this;
    }

    public StringGuard IsOneOf(HashSet<string> validValues, string? message = null)
    {
        if (value is not null && !validValues.Contains(value)) throw new ArgumentException(message ?? $"Ungültiger Wert für {paramName}: {value}.", paramName);
        return this;
    }

    public StringGuard MatchesPattern(Regex pattern, string? message = null)
    {
        if (value is not null && !pattern.IsMatch(value)) throw new ArgumentException(message ?? $"{paramName} hat ein ungültiges Format.", paramName);
        return this;
    }
}
