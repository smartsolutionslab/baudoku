using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

public sealed record ChunkCount : IValueObject
{
    public int Value { get; }

    private ChunkCount(int value) => Value = value;

    public static ChunkCount From(int value)
    {
        Ensure.That(value).IsPositive("Chunk-Anzahl muss groesser als 0 sein.");
        return new ChunkCount(value);
    }
}
