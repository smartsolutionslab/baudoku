using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain;

public sealed record InstallationStatus : IValueObject
{
    private static readonly HashSet<string> ValidValues = ["in_progress", "completed", "inspected"];

    public static readonly InstallationStatus InProgress = new("in_progress");
    public static readonly InstallationStatus Completed = new("completed");
    public static readonly InstallationStatus Inspected = new("inspected");

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
