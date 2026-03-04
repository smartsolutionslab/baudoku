using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Projects.Domain;

public sealed record ZoneType : IValueObject
{
    public static readonly ZoneType Building = new("building");
    public static readonly ZoneType Floor = new("floor");
    public static readonly ZoneType Room = new("room");
    public static readonly ZoneType Trench = new("trench");

    public static IEnumerable<ZoneType> All { get; } =
    [
        Building,
        Floor,
        Room,
        Trench
    ];

    private static HashSet<string> ValidValues => All.Select(item => item.Value).ToHashSet();

    public string Value { get; }

    private ZoneType(string value) => Value = value;

    public static ZoneType From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Zonentyp darf nicht leer sein.")
            .IsOneOf(ValidValues, $"Ungültiger Zonentyp: {value}.");
        return new ZoneType(value);
    }
}
