using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Queries;
using SmartSolutionsLab.BauDoku.Sync.Application.Queries;
using SmartSolutionsLab.BauDoku.Sync.ReadModel;

namespace SmartSolutionsLab.BauDoku.Sync.Application.Queries.Handlers;

public sealed class GetConflictsQueryHandler(ISyncBatchReadRepository syncBatches)
    : IQueryHandler<GetConflictsQuery, List<ConflictDto>>
{
    public async Task<List<ConflictDto>> Handle(GetConflictsQuery query, CancellationToken cancellationToken = default)
    {
        var (deviceId, status) = query;
        return await syncBatches.GetConflictsAsync(deviceId, status, cancellationToken);
    }
}
