using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Sync.Domain;

public sealed record BatchStatus : IValueObject
{
    public static readonly BatchStatus Pending = new("pending");
    public static readonly BatchStatus Processing = new("processing");
    public static readonly BatchStatus Completed = new("completed");
    public static readonly BatchStatus PartialConflict = new("partial_conflict");
    public static readonly BatchStatus Failed = new("failed");

    public static IEnumerable<BatchStatus> All { get; } =
    [
        Pending,
        Processing,
        Completed,
        PartialConflict,
        Failed
    ];

    private static HashSet<string> ValidValues => All.Select(item => item.Value).ToHashSet();


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
