using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Sync.Application.Queries.Dtos;

namespace BauDoku.Sync.Application.Queries.GetConflicts;

public sealed record GetConflictsQuery(
    string? DeviceId,
    string? Status) : IQuery<List<ConflictDto>>;
