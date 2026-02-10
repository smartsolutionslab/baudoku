using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.Domain.Events;

public sealed record ProjectCreated(ProjectId ProjectId, ProjectName Name, DateTime OccurredOn) : IDomainEvent;
