using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record ConflictRecordIdentifier : IValueObject
{
    public Guid Value { get; }

    private ConflictRecordIdentifier(Guid value) => Value = value;

    public static ConflictRecordIdentifier From(Guid value)
    {
        Ensure.That(value).IsNotEmpty("ConflictRecord-ID darf nicht leer sein.");
        return new ConflictRecordIdentifier(value);
    }

    public static ConflictRecordIdentifier New() => new(Guid.NewGuid());
}
