using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record FileName : IValueObject
{
    public const int MaxLength = 255;

    public string Value { get; }

    private FileName(string value) => Value = value;

    public static FileName From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("Dateiname darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Dateiname darf max. {MaxLength} Zeichen lang sein.");
        return new FileName(value);
    }
}
