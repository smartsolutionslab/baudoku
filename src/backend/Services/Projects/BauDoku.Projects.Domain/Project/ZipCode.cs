using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Projects.Domain;

public sealed record ZipCode : IValueObject
{
    public const int MaxLength = 10;
    public string Value { get; }

    private ZipCode(string value) => Value = value;

    public static ZipCode From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("PLZ darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"PLZ darf max. {MaxLength} Zeichen lang sein.");
        return new ZipCode(value);
    }
}
