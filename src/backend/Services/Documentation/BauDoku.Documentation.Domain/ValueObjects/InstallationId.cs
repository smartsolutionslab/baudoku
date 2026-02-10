using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record InstallationId : ValueObject
{
    public Guid Value { get; }

    public InstallationId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Installations-ID darf nicht leer sein.", nameof(value));
        Value = value;
    }

    public static InstallationId New() => new(Guid.NewGuid());
}
