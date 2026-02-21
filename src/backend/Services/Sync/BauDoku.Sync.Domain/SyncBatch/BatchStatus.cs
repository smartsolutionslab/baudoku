using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Sync.Domain;

public sealed record BatchStatus : IValueObject
{
    private static readonly HashSet<string> ValidValues =
        ["pending", "processing", "completed", "partial_conflict", "failed"];

    public static readonly BatchStatus Pending = new("pending");
    public static readonly BatchStatus Processing = new("processing");
    public static readonly BatchStatus Completed = new("completed");
    public static readonly BatchStatus PartialConflict = new("partial_conflict");
    public static readonly BatchStatus Failed = new("failed");

    public string Value { get; }

    private BatchStatus(string value) => Value = value;

    public static BatchStatus From(string value)
    {
        Ensure.That(value)
            .IsNotNullOrWhiteSpace("Batch-Status darf nicht leer sein.")
            .IsOneOf(ValidValues, $"Ungültiger Batch-Status: {value}.");
        return new BatchStatus(value);
    }
}
