using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record Depth : IValueObject
{
    public int Value { get; }

    private Depth(int value) => Value = value;

    public static Depth From(int value)
    {
        Ensure.That(value).IsNotNegative("Tiefe darf nicht negativ sein.");
        return new Depth(value);
    }

    public static Depth? FromNullable(int? value) => value.HasValue ? From(value.Value) : null;
}
