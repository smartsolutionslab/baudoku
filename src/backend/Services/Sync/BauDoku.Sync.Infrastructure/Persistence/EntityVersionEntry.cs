namespace BauDoku.Sync.Infrastructure.Persistence;

public sealed class EntityVersionEntry
{
    public string EntityType { get; set; } = default!;
    public Guid EntityId { get; set; }
    public long Version { get; set; }
    public string Payload { get; set; } = default!;
    public DateTime LastModified { get; set; }
    public string LastDeviceId { get; set; } = default!;
    public string Operation { get; set; } = "update";
    public uint RowVersion { get; set; }
}
