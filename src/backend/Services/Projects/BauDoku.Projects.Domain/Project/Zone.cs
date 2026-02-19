using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.Domain;

public sealed class Zone : Entity<ZoneIdentifier>
{
    public ZoneName Name { get; private set; } = default!;
    public ZoneType Type { get; private set; } = default!;
    public ZoneIdentifier? ParentZoneIdentifier { get; private set; }

    private Zone() { } // EF Core

    internal static Zone Create(ZoneIdentifier id, ZoneName name, ZoneType type, ZoneIdentifier? parentZoneIdentifier = null)
    {
        return new Zone
        {
            Id = id,
            Name = name,
            Type = type,
            ParentZoneIdentifier = parentZoneIdentifier
        };
    }
}
