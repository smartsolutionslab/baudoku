using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Sync.Domain;

public sealed record DeltaOperation : IValueObject
{
    private static readonly HashSet<string> ValidValues = ["create", "update", "delete"];

    public static readonly DeltaOperation Create = new("create");
    public static readonly DeltaOperation Update = new("update");
    public static readonly DeltaOperation Delete = new("delete");

    public string Value { get; }

    private DeltaOperation(string value) => Value = value;

    public static DeltaOperation From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Delta-Operation darf nicht leer sein.")
            .IsOneOf(ValidValues, $"Ungueltige Delta-Operation: {value}.");
        return new DeltaOperation(value);
    }
}
