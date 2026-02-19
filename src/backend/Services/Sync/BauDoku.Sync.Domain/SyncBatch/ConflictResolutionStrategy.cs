using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record ConflictResolutionStrategy : IValueObject
{
    private static readonly HashSet<string> ValidValues = ["client_wins", "server_wins", "manual_merge"];

    public static readonly ConflictResolutionStrategy ClientWins = new("client_wins");
    public static readonly ConflictResolutionStrategy ServerWins = new("server_wins");
    public static readonly ConflictResolutionStrategy ManualMerge = new("manual_merge");

    public string Value { get; }

    private ConflictResolutionStrategy(string value) => Value = value;

    public static ConflictResolutionStrategy From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Konflikt-Auflösungsstrategie darf nicht leer sein.")
            .IsOneOf(ValidValues, $"Ungueltige Konflikt-Auflösungsstrategie: {value}.");
        return new ConflictResolutionStrategy(value);
    }
}
