using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record EntityType : ValueObject
{
    private static readonly HashSet<string> ValidValues =
        ["project", "zone", "installation", "photo", "measurement"];

    public static readonly EntityType Project = new("project");
    public static readonly EntityType Zone = new("zone");
    public static readonly EntityType Installation = new("installation");
    public static readonly EntityType Photo = new("photo");
    public static readonly EntityType Measurement = new("measurement");

    public string Value { get; }

    public EntityType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Entity-Typ darf nicht leer sein.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException($"Ungueltiger Entity-Typ: {value}.", nameof(value));
        Value = value;
    }
}
