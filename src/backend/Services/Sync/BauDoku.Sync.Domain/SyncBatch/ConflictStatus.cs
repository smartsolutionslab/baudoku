using System.Runtime.CompilerServices;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Sync.Domain;

public sealed record ConflictStatus : IValueObject
{
    public static readonly ConflictStatus Unresolved = new("unresolved");
    public static readonly ConflictStatus ClientWins = new("client_wins");
    public static readonly ConflictStatus ServerWins = new("server_wins");
    public static readonly ConflictStatus Merged = new("merged");

    public static IEnumerable<ConflictStatus> All { get; } =
    [
        Unresolved,
        ClientWins,
        ServerWins,
        Merged
    ];

    private static HashSet<string> ValidValues => All.Select(item => item.Value).ToHashSet();

    public string Value { get; }

    private ConflictStatus(string value) => Value = value;

    public static ConflictStatus From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Konflikt-Status darf nicht leer sein.")
            .IsOneOf(ValidValues, $"Ungültiger Konflikt-Status: {value}.");
        return new ConflictStatus(value);
    }

    public static ConflictStatus? FromNullable(string? value) => value is not null ? From(value) : null;
}
