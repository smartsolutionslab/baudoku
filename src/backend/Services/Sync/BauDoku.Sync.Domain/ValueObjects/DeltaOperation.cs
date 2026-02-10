using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record DeltaOperation : ValueObject
{
    private static readonly HashSet<string> ValidValues =
        ["create", "update", "delete"];

    public static readonly DeltaOperation Create = new("create");
    public static readonly DeltaOperation Update = new("update");
    public static readonly DeltaOperation Delete = new("delete");

    public string Value { get; }

    public DeltaOperation(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Delta-Operation darf nicht leer sein.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException($"Ungueltige Delta-Operation: {value}.", nameof(value));
        Value = value;
    }
}
