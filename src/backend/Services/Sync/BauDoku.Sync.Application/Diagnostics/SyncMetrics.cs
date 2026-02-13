using System.Diagnostics.Metrics;

namespace BauDoku.Sync.Application.Diagnostics;

public static class SyncMetrics
{
    private static readonly Meter Meter = new("BauDoku.Sync");

    public static readonly Counter<long> BatchesProcessed =
        Meter.CreateCounter<long>("baudoku.sync.batches_processed", description: "Number of sync batches processed");

    public static readonly Counter<long> DeltasApplied =
        Meter.CreateCounter<long>("baudoku.sync.deltas_applied", description: "Number of sync deltas applied");

    public static readonly Counter<long> ConflictsDetected =
        Meter.CreateCounter<long>("baudoku.sync.conflicts_detected", description: "Number of sync conflicts detected");

    public static readonly Counter<long> ConflictsResolved =
        Meter.CreateCounter<long>("baudoku.sync.conflicts_resolved", description: "Number of sync conflicts resolved");

    public static readonly Histogram<long> DeltaPayloadSize =
        Meter.CreateHistogram<long>("baudoku.sync.delta_payload_size_bytes", unit: "By", description: "Size of individual sync delta payloads in bytes");

    public static readonly Histogram<int> DeltasPerBatch =
        Meter.CreateHistogram<int>("baudoku.sync.deltas_per_batch", description: "Number of deltas per sync batch");
}
