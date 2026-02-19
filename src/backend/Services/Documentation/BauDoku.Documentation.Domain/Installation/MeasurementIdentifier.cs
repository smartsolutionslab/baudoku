using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record MeasurementIdentifier : IValueObject
{
    public Guid Value { get; }

    private MeasurementIdentifier(Guid value) => Value = value;

    public static MeasurementIdentifier From(Guid value)
    {
        Ensure.That(value).IsNotEmpty("Messungs-ID darf nicht leer sein.");
        return new MeasurementIdentifier(value);
    }

    public static MeasurementIdentifier New() => new(Guid.NewGuid());
}
