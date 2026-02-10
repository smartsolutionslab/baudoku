using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.Domain.ValueObjects;

public sealed record ZoneId : ValueObject
{
    public Guid Value { get; }

    public ZoneId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Zonen-ID darf nicht leer sein.", nameof(value));
        Value = value;
    }

    public static ZoneId New() => new(Guid.NewGuid());
}
