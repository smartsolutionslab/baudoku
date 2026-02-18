using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record ConflictStatus : IValueObject
{
    private static readonly HashSet<string> ValidValues = ["unresolved", "client_wins", "server_wins", "merged"];

    public static readonly ConflictStatus Unresolved = new("unresolved");
    public static readonly ConflictStatus ClientWins = new("client_wins");
    public static readonly ConflictStatus ServerWins = new("server_wins");
    public static readonly ConflictStatus Merged = new("merged");

    public string Value { get; }

    private ConflictStatus(string value) => Value = value;

    public static ConflictStatus From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Konflikt-Status darf nicht leer sein.")
            .IsOneOf(ValidValues, $"Ungültiger Konflikt-Status: {value}.");
        return new ConflictStatus(value);
    }
}
