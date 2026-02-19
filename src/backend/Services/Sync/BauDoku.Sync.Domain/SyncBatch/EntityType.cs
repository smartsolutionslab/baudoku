using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record EntityType : IValueObject
{
    private static readonly HashSet<string> ValidValues = ["project", "zone", "installation", "photo", "measurement"];

    public static readonly EntityType Project = new("project");
    public static readonly EntityType Zone = new("zone");
    public static readonly EntityType Installation = new("installation");
    public static readonly EntityType Photo = new("photo");
    public static readonly EntityType Measurement = new("measurement");

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
