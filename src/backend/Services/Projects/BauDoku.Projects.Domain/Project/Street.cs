using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Projects.Domain.ValueObjects;

public sealed record Street : IValueObject
{
    public const int MaxLength = 200;
    public string Value { get; }

    private Street(string value) => Value = value;

    public static Street From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Straße darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Straße darf max. {MaxLength} Zeichen lang sein.");
        return new Street(value);
    }
}
