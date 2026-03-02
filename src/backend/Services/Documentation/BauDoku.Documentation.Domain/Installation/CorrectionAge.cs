using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain;

public sealed record CorrectionAge : IValueObject
{
    public double Value { get; }

    private CorrectionAge(double value) => Value = value;

    public static CorrectionAge From(double value)
    {
        Ensure.That(value).IsNotNegative("Korrekturalter darf nicht negativ sein.");
        return new CorrectionAge(value);
    }

    public static CorrectionAge? FromNullable(double? value) => value.HasValue ? From(value.Value) : null;
}
