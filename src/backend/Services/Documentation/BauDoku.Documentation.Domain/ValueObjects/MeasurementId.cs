using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record MeasurementId : ValueObject
{
    public Guid Value { get; }

    public MeasurementId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Messungs-ID darf nicht leer sein.", nameof(value));
        Value = value;
    }

    public static MeasurementId New() => new(Guid.NewGuid());
}
