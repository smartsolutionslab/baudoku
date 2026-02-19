using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Projects.Domain;

public sealed record ZoneAdded(
    ProjectIdentifier ProjectIdentifier,
    ZoneIdentifier ZoneIdentifier,
    ZoneName Name,
    ZoneType Type,
    DateTime OccurredOn) : IDomainEvent;
