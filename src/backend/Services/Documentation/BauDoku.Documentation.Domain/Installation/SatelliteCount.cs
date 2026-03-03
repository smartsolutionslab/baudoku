using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record SatelliteCount : IValueObject
{
    public int Value { get; }

    private SatelliteCount(int value) => Value = value;

    public static SatelliteCount From(int value)
    {
        Ensure.That(value).IsNotNegative("Satellitenanzahl darf nicht negativ sein.");
        return new SatelliteCount(value);
    }

    public static SatelliteCount? FromNullable(int? value) => value.HasValue ? From(value.Value) : null;
}
