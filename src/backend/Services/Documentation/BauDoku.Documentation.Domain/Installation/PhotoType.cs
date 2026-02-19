using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain;

public sealed record PhotoType : IValueObject
{
    private static readonly HashSet<string> ValidValues = ["before", "after", "detail", "overview", "document", "other"];

    public static readonly PhotoType Before = new("before");
    public static readonly PhotoType After = new("after");
    public static readonly PhotoType Detail = new("detail");
    public static readonly PhotoType Overview = new("overview");
    public static readonly PhotoType Document = new("document");
    public static readonly PhotoType Other = new("other");

    public string Value { get; }

    private PhotoType(string value) => Value = value;

    public static PhotoType From(string value)
    {
        Ensure.That(value).IsNotNullOrWhiteSpace("Fototyp darf nicht leer sein.")
            .IsOneOf(ValidValues, $"Ungültiger Fototyp: {value}.");
        return new PhotoType(value);
    }
}
