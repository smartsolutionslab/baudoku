using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record ConductorCount : IValueObject
{
    public int Value { get; }

    private ConductorCount(int value) => Value = value;

    public static ConductorCount From(int value)
    {
        Ensure.That(value).IsPositive("Leiteranzahl muss groesser als 0 sein.");
        return new ConductorCount(value);
    }

    public static ConductorCount? FromNullable(int? value) => value.HasValue ? From(value.Value) : null;
}
