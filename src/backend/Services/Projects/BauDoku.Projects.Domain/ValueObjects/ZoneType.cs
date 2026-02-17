using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Projects.Domain.ValueObjects;

public sealed record ZoneType : IValueObject
{
    private static readonly HashSet<string> ValidValues = ["building", "floor", "room", "trench"];

    public static readonly ZoneType Building = new("building");
    public static readonly ZoneType Floor = new("floor");
    public static readonly ZoneType Room = new("room");
    public static readonly ZoneType Trench = new("trench");

    public string Value { get; }

    private ZoneType(string value) => Value = value;

    public static ZoneType From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Zonentyp darf nicht leer sein.")
            .IsOneOf(ValidValues, $"Ungueltiger Zonentyp: {value}.");
        return new ZoneType(value);
    }
}
