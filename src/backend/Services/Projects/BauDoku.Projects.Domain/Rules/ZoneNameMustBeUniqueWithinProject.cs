using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Domain.Entities;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Domain.Rules;

public sealed class ZoneNameMustBeUniqueWithinProject : IBusinessRule
{
    private readonly IReadOnlyList<Zone> existingZones;
    private readonly ZoneName newZoneName;

    public ZoneNameMustBeUniqueWithinProject(IReadOnlyList<Zone> existingZones, ZoneName newZoneName)
    {
        this.existingZones = existingZones;
        this.newZoneName = newZoneName;
    }

    public bool IsBroken() =>
        existingZones.Any(z => z.Name == newZoneName);

    public string Message => $"Zone mit dem Namen '{newZoneName.Value}' existiert bereits im Projekt.";
}
