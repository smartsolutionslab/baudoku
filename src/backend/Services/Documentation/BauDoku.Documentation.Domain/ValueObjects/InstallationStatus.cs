using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record InstallationStatus : ValueObject
{
    private static readonly HashSet<string> ValidValues =
        ["in_progress", "completed", "inspected"];

    public static readonly InstallationStatus InProgress = new("in_progress");
    public static readonly InstallationStatus Completed = new("completed");
    public static readonly InstallationStatus Inspected = new("inspected");

    public string Value { get; }

    public InstallationStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Installationsstatus darf nicht leer sein.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException($"Ungueltiger Installationsstatus: {value}.", nameof(value));
        Value = value;
    }
}
