using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record InstallationIdentifier : IValueObject
{
    public Guid Value { get; }

    private InstallationIdentifier(Guid value) => Value = value;

    public static InstallationIdentifier From(Guid value)
    {
        Ensure.That(value).IsNotEmpty("Installations-ID darf nicht leer sein.");
        return new InstallationIdentifier(value);
    }

    public static InstallationIdentifier New() => new(Guid.NewGuid());
}
