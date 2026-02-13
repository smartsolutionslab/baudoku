using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Domain.Entities;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Domain.Rules;

public sealed class ZoneNameMustBeUniqueWithinProject : IBusinessRule
{
    private readonly IReadOnlyList<Zone> existingZones;
    private readonly ZoneName newZoneName;
    private readonly ZoneIdentifier? parentZoneIdentifier;

    public ZoneNameMustBeUniqueWithinProject(
        IReadOnlyList<Zone> existingZones,
        ZoneName newZoneName,
        ZoneIdentifier? parentZoneIdentifier = null)
    {
        this.existingZones = existingZones;
        this.newZoneName = newZoneName;
        this.parentZoneIdentifier = parentZoneIdentifier;
    }

    public bool IsBroken() =>
        existingZones.Any(z => z.Name == newZoneName && z.ParentZoneIdentifier == parentZoneIdentifier);

    public string Message => $"Zone mit dem Namen '{newZoneName.Value}' existiert bereits im Projekt.";
}
