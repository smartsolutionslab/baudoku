using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain;

public sealed record GpsSource : IValueObject
{
    public const int MaxLength = 20;

    public string Value { get; }

    private GpsSource(string value) => Value = value;

    public static GpsSource From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("GPS-Quelle darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"GPS-Quelle darf max. {MaxLength} Zeichen lang sein.");
        return new GpsSource(value);
    }
}
