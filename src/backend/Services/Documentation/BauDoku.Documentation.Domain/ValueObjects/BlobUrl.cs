using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record BlobUrl : IValueObject
{
    public const int MaxLength = 2000;

    public string Value { get; }

    private BlobUrl(string value) => Value = value;

    public static BlobUrl From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("Blob-URL darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Blob-URL darf max. {MaxLength} Zeichen lang sein.");
        return new BlobUrl(value);
    }
}
