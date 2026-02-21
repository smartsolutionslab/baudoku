using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.BuildingBlocks.Domain;

public sealed record SearchTerm : IValueObject
{
    public const int MaxLength = 200;

    public string Value { get; }

    private SearchTerm(string value) => Value = value;

    public static SearchTerm From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Suchbegriff darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Suchbegriff darf max. {MaxLength} Zeichen lang sein.");
        return new SearchTerm(value);
    }

    public static SearchTerm? FromNullable(string? value) => value is not null ? From(value) : null;
}
