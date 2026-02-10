using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.Domain.ValueObjects;

public sealed record ZoneType : ValueObject
{
    private static readonly HashSet<string> ValidValues = ["building", "floor", "room", "trench"];

    public static readonly ZoneType Building = new("building");
    public static readonly ZoneType Floor = new("floor");
    public static readonly ZoneType Room = new("room");
    public static readonly ZoneType Trench = new("trench");

    public string Value { get; }

    public ZoneType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Zonentyp darf nicht leer sein.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException($"Ungültiger Zonentyp: {value}.", nameof(value));
        Value = value;
    }
}
