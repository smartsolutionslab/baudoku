using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Domain.Events;

public sealed record ZoneAdded(ProjectIdentifier ProjectIdentifier, ZoneIdentifier ZoneIdentifier, ZoneName Name, ZoneType Type, DateTime OccurredOn) : IDomainEvent;
