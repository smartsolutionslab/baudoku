using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Queries;
using SmartSolutionsLab.BauDoku.Sync.Domain;

namespace SmartSolutionsLab.BauDoku.Sync.Application.Queries;

public sealed record GetChangesSinceQuery(
    DeviceIdentifier DeviceId,
    SyncLimit Limit,
    DateTime? Since = null) : IQuery<ChangeSetResult>;
