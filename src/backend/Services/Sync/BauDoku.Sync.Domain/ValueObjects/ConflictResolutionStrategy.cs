using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record ConflictResolutionStrategy : ValueObject
{
    private static readonly HashSet<string> ValidValues =
        ["client_wins", "server_wins", "manual_merge"];

    public static readonly ConflictResolutionStrategy ClientWins = new("client_wins");
    public static readonly ConflictResolutionStrategy ServerWins = new("server_wins");
    public static readonly ConflictResolutionStrategy ManualMerge = new("manual_merge");

    public string Value { get; }

    public ConflictResolutionStrategy(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Konflikt-Auflösungsstrategie darf nicht leer sein.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException($"Ungueltige Konflikt-Auflösungsstrategie: {value}.", nameof(value));
        Value = value;
    }
}
