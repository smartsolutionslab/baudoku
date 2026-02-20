using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Application.Queries;
using BauDoku.Sync.Application.Queries.Dtos;

namespace BauDoku.Sync.Application.Queries.Handlers;

public sealed class GetConflictsQueryHandler(ISyncBatchReadRepository syncBatches)
    : IQueryHandler<GetConflictsQuery, List<ConflictDto>>
{
    public async Task<List<ConflictDto>> Handle(GetConflictsQuery query, CancellationToken cancellationToken = default)
    {
        var (deviceId, status) = query;
        return await syncBatches.GetConflictsAsync(deviceId, status, cancellationToken);
    }
}
