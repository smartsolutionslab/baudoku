using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain;

public sealed record PhotoIdentifier : IValueObject
{
    public Guid Value { get; }

    private PhotoIdentifier(Guid value) => Value = value;

    public static PhotoIdentifier From(Guid value)
    {
        Ensure.That(value).IsNotEmpty("Foto-ID darf nicht leer sein.");
        return new PhotoIdentifier(value);
    }

    public static PhotoIdentifier New() => new(Guid.NewGuid());
}
