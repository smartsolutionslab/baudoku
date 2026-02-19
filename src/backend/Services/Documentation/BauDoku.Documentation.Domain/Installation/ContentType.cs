using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain;

public sealed record ContentType : IValueObject
{
    public const int MaxLength = 50;

    public string Value { get; }

    private ContentType(string value) => Value = value;

    public static ContentType From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("Content-Type darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Content-Type darf max. {MaxLength} Zeichen lang sein.");
        return new ContentType(value);
    }
}
