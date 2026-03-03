using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Projects.Domain;

public sealed record ProjectStatus : IValueObject
{
    private static readonly HashSet<string> ValidValues = ["draft", "active", "completed", "archived"];

    public static readonly ProjectStatus Draft = new("draft");
    public static readonly ProjectStatus Active = new("active");
    public static readonly ProjectStatus Completed = new("completed");
    public static readonly ProjectStatus Archived = new("archived");

    public string Value { get; }

    private ProjectStatus(string value) => Value = value;

    public static ProjectStatus From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Projektstatus darf nicht leer sein.")
            .IsOneOf(ValidValues, $"Ungültiger Projektstatus: {value}.");
        return new ProjectStatus(value);
    }
}
