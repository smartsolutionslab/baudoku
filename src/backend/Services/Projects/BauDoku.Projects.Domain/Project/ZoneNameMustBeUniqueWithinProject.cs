using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Domain.Entities;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Domain.Rules;

public sealed class ZoneNameMustBeUniqueWithinProject(IReadOnlyList<Zone> existingZones, ZoneName newZoneName, ZoneIdentifier? parentZoneIdentifier = null)
    : IBusinessRule
{
    public bool IsBroken() => existingZones.Any(z => z.Name == newZoneName && z.ParentZoneIdentifier == parentZoneIdentifier);

    public string Message => $"Zone mit dem Namen '{newZoneName.Value}' existiert bereits im Projekt.";
}
