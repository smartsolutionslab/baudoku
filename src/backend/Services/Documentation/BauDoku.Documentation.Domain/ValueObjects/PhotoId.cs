using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record PhotoId : ValueObject
{
    public Guid Value { get; }

    public PhotoId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Foto-ID darf nicht leer sein.", nameof(value));
        Value = value;
    }

    public static PhotoId New() => new(Guid.NewGuid());
}
