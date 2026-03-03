using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Projects.Domain;

public sealed record ClientName : IValueObject
{
    public const int MaxLength = 200;
    public string Value { get; }

    private ClientName(string value) => Value = value;

    public static ClientName From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Kundenname darf nicht leer sein.")
            .MaxLengthIs(MaxLength, $"Kundenname darf max. {MaxLength} Zeichen lang sein.");
        return new ClientName(value);
    }
}
