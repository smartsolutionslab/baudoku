using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record RtkFixStatus : IValueObject
{
    public const int MaxLength = 10;

    public string Value { get; }

    private RtkFixStatus(string value) => Value = value;

    public static RtkFixStatus From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("RTK-Fix-Status darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"RTK-Fix-Status darf max. {MaxLength} Zeichen lang sein.");
        return new RtkFixStatus(value);
    }
}
