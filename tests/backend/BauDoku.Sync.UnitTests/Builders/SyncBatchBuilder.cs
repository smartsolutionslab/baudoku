using BauDoku.Sync.Domain.Aggregates;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.UnitTests.Builders;

internal sealed class SyncBatchBuilder
{
    private SyncBatchId _id = SyncBatchId.New();
    private DeviceId _deviceId = new("device-001");
    private DateTime _submittedAt = DateTime.UtcNow;

    public SyncBatchBuilder WithId(SyncBatchId id) { _id = id; return this; }
    public SyncBatchBuilder WithDeviceId(DeviceId deviceId) { _deviceId = deviceId; return this; }
    public SyncBatchBuilder WithSubmittedAt(DateTime submittedAt) { _submittedAt = submittedAt; return this; }

    public SyncBatch Build() => SyncBatch.Create(_id, _deviceId, _submittedAt);
}
