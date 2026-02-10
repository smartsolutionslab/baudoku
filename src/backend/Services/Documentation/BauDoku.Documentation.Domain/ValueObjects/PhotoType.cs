using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record PhotoType : ValueObject
{
    private static readonly HashSet<string> ValidValues =
    [
        "before", "after", "detail", "overview", "document", "other"
    ];

    public static readonly PhotoType Before = new("before");
    public static readonly PhotoType After = new("after");
    public static readonly PhotoType Detail = new("detail");
    public static readonly PhotoType Overview = new("overview");
    public static readonly PhotoType Document = new("document");
    public static readonly PhotoType Other = new("other");

    public string Value { get; }

    public PhotoType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Fototyp darf nicht leer sein.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException($"Ungueltiger Fototyp: {value}.", nameof(value));
        Value = value;
    }
}
