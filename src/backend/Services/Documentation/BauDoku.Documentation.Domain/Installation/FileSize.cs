using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record FileSize : IValueObject
{
    public long Value { get; }

    private FileSize(long value) => Value = value;

    public static FileSize From(long value)
    {
        Ensure.That(value).IsPositive("Dateigröße muss größer als 0 sein.");
        return new FileSize(value);
    }
}
