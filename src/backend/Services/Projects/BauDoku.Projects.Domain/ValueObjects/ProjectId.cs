using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.Domain.ValueObjects;

public sealed record ProjectId : ValueObject
{
    public Guid Value { get; }

    public ProjectId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Projekt-ID darf nicht leer sein.", nameof(value));
        Value = value;
    }

    public static ProjectId New() => new(Guid.NewGuid());
}
