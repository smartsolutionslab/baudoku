using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Application.Queries.GetConflicts;

public sealed class GetConflictsQueryHandler(ISyncBatchReadRepository syncBatches)
    : IQueryHandler<GetConflictsQuery, List<ConflictDto>>
{
    public async Task<List<ConflictDto>> Handle(GetConflictsQuery query, CancellationToken cancellationToken = default)
    {
        var (deviceId, status) = query;
        return await syncBatches.GetConflictsAsync(
            deviceId is not null ? DeviceIdentifier.From(deviceId) : null,
            status is not null ? ConflictStatus.From(status) : null,
            cancellationToken);
    }
}
