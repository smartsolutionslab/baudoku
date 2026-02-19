using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Projects.Domain;

public sealed record City : IValueObject
{
    public const int MaxLength = 100;
    public string Value { get; }

    private City(string value) => Value = value;

    public static City From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Stadt darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Stadt darf max. {MaxLength} Zeichen lang sein.");
        return new City(value);
    }
}
