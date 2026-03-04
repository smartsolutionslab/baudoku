using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record InstallationStatus : IValueObject
{
    public static readonly InstallationStatus InProgress = new("in_progress");
    public static readonly InstallationStatus Completed = new("completed");
    public static readonly InstallationStatus Inspected = new("inspected");

    public static IEnumerable<InstallationStatus> All { get; } =
    [
        InProgress,
        Completed,
        Inspected,
    ];

    private static HashSet<string> ValidValues => All.Select(item => item.Value).ToHashSet();

    public string Value { get; }

    private InstallationStatus(string value) => Value = value;

    public static InstallationStatus From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("Installationsstatus darf nicht leer sein.")
            .IsOneOf(ValidValues, $"Ungültiger Installationsstatus: {value}.");
        return new InstallationStatus(value);
    }

    public static InstallationStatus? FromNullable(string? value) => value is not null ? From(value) : null;
}
