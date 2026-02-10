using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record ConflictRecordId : ValueObject
{
    public Guid Value { get; }

    public ConflictRecordId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ConflictRecord-ID darf nicht leer sein.", nameof(value));
        Value = value;
    }

    public static ConflictRecordId New() => new(Guid.NewGuid());
}
