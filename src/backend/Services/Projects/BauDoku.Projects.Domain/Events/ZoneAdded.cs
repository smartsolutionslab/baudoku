using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Domain.Events;

public sealed record ZoneAdded(ProjectId ProjectId, ZoneId ZoneId, ZoneName Name, ZoneType Type, DateTime OccurredOn) : IDomainEvent;
