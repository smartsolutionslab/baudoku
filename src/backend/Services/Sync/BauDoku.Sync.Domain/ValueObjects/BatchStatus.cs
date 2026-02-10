using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Sync.Domain.ValueObjects;

public sealed record BatchStatus : ValueObject
{
    private static readonly HashSet<string> ValidValues =
        ["pending", "processing", "completed", "partial_conflict", "failed"];

    public static readonly BatchStatus Pending = new("pending");
    public static readonly BatchStatus Processing = new("processing");
    public static readonly BatchStatus Completed = new("completed");
    public static readonly BatchStatus PartialConflict = new("partial_conflict");
    public static readonly BatchStatus Failed = new("failed");

    public string Value { get; }

    public BatchStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Batch-Status darf nicht leer sein.", nameof(value));
        if (!ValidValues.Contains(value))
            throw new ArgumentException($"Ungueltiger Batch-Status: {value}.", nameof(value));
        Value = value;
    }
}
