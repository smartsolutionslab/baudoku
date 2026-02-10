using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Domain.Entities;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Domain.Rules;

public sealed class ZoneNameMustBeUniqueWithinProject : IBusinessRule
{
    private readonly IReadOnlyList<Zone> _existingZones;
    private readonly ZoneName _newZoneName;

    public ZoneNameMustBeUniqueWithinProject(IReadOnlyList<Zone> existingZones, ZoneName newZoneName)
    {
        _existingZones = existingZones;
        _newZoneName = newZoneName;
    }

    public bool IsBroken() =>
        _existingZones.Any(z => z.Name == _newZoneName);

    public string Message => $"Zone mit dem Namen '{_newZoneName.Value}' existiert bereits im Projekt.";
}
