using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Sync.Domain;

public sealed record EntityType : IValueObject
{
    public static readonly EntityType Project = new("project");
    public static readonly EntityType Zone = new("zone");
    public static readonly EntityType Installation = new("installation");
    public static readonly EntityType Photo = new("photo");
    public static readonly EntityType Measurement = new("measurement");

    public static IEnumerable<EntityType> All { get; } =
    [
        Project,
        Zone,
        Installation,
        Photo,
        Measurement
    ];

    private static HashSet<string> ValidValues => All.Select(item => item.Value).ToHashSet();

    public string Value { get; }

    private EntityType(string value) => Value = value;

    public static EntityType From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Entity-Typ darf nicht leer sein.")
            .IsOneOf(ValidValues, $"Ungültiger Entity-Typ: {value}.");
        return new EntityType(value);
    }
}
