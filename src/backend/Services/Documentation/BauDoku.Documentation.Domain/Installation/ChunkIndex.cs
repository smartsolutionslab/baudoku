using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain;

public sealed record ChunkIndex : IValueObject
{
    public int Value { get; }

    private ChunkIndex(int value) => Value = value;

    public static ChunkIndex From(int value)
    {
        Ensure.That(value).IsNotNegative("Chunk-Index darf nicht negativ sein.");
        return new ChunkIndex(value);
    }
}
