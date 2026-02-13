using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Projects.Domain.ValueObjects;

public sealed record ProjectIdentifier : ValueObject
{
    public Guid Value { get; }

    private ProjectIdentifier(Guid value) => Value = value;

    public static ProjectIdentifier From(Guid value)
    {
        Ensure.That(value).IsNotEmpty("Projekt-ID darf nicht leer sein.");
        return new ProjectIdentifier(value);
    }

    public static ProjectIdentifier New() => new(Guid.NewGuid());
}
