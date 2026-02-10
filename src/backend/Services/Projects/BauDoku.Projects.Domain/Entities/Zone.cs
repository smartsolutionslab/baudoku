using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Domain.Entities;

public sealed class Zone : Entity<ZoneId>
{
    public ZoneName Name { get; private set; } = default!;
    public ZoneType Type { get; private set; } = default!;
    public ZoneId? ParentZoneId { get; private set; }

    private Zone() { } // EF Core

    internal static Zone Create(ZoneId id, ZoneName name, ZoneType type, ZoneId? parentZoneId = null)
    {
        return new Zone
        {
            Id = id,
            Name = name,
            Type = type,
            ParentZoneId = parentZoneId
        };
    }
}
