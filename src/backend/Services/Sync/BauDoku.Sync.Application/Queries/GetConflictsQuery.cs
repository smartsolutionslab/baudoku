using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Queries;
using SmartSolutionsLab.BauDoku.Sync.ReadModel;
using SmartSolutionsLab.BauDoku.Sync.Domain;

namespace SmartSolutionsLab.BauDoku.Sync.Application.Queries;

public sealed record GetConflictsQuery(
    DeviceIdentifier? DeviceId,
    ConflictStatus? Status) : IQuery<List<ConflictDto>>;
