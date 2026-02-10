using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record ConflictStatus : ValueObject
{
    private static readonly HashSet<string> ValidValues =
        ["unresolved", "client_wins", "server_wins", "merged"];

    public static readonly ConflictStatus Unresolved = new("unresolved");
    public static readonly ConflictStatus ClientWins = new("client_wins");
    public static readonly ConflictStatus ServerWins = new("server_wins");
    public static readonly ConflictStatus Merged = new("merged");

    public string Value { get; }

    public ConflictStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Konflikt-Status darf nicht leer sein.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException($"Ungueltiger Konflikt-Status: {value}.", nameof(value));
        Value = value;
    }
}
