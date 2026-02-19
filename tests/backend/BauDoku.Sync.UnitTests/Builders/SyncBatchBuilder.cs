using BauDoku.Sync.Domain;

namespace BauDoku.Sync.UnitTests.Builders;

internal sealed class SyncBatchBuilder
{
    private SyncBatchIdentifier id = SyncBatchIdentifier.New();
    private DeviceIdentifier deviceId = DeviceIdentifier.From("device-001");
    private DateTime submittedAt = DateTime.UtcNow;

    public SyncBatchBuilder WithId(SyncBatchIdentifier value) { id = value; return this; }
    public SyncBatchBuilder WithDeviceId(DeviceIdentifier value) { deviceId = value; return this; }
    public SyncBatchBuilder WithSubmittedAt(DateTime value) { submittedAt = value; return this; }

    public SyncBatch Build() => SyncBatch.Create(id, deviceId, submittedAt);
}
