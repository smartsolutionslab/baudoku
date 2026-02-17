using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Projects.Domain.ValueObjects;

public sealed record ZoneIdentifier : IValueObject
{
    public Guid Value { get; }

    private ZoneIdentifier(Guid value) => Value = value;

    public static ZoneIdentifier From(Guid value)
    {
        Ensure.That(value).IsNotEmpty("Zonen-ID darf nicht leer sein.");
        return new ZoneIdentifier(value);
    }

    public static ZoneIdentifier New() => new(Guid.NewGuid());
}
