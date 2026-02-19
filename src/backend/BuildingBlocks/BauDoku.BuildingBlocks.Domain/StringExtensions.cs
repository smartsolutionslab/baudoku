namespace BauDoku.BuildingBlocks.Domain;

public static class StringExtensions
{
    public static bool HasValue(this string? value) => !string.IsNullOrWhiteSpace(value);
}
