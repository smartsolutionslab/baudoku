using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.Domain.ValueObjects;

public sealed record ProjectStatus : ValueObject
{
    private static readonly HashSet<string> ValidValues = ["draft", "active", "completed", "archived"];

    public static readonly ProjectStatus Draft = new("draft");
    public static readonly ProjectStatus Active = new("active");
    public static readonly ProjectStatus Completed = new("completed");
    public static readonly ProjectStatus Archived = new("archived");

    public string Value { get; }

    public ProjectStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Projektstatus darf nicht leer sein.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException($"Ungültiger Projektstatus: {value}.", nameof(value));
        Value = value;
    }
}
