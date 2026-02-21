using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Domain;

namespace BauDoku.Sync.Application.Queries;

public sealed record GetConflictsQuery(
    DeviceIdentifier? DeviceId,
    ConflictStatus? Status) : IQuery<List<ConflictDto>>;
