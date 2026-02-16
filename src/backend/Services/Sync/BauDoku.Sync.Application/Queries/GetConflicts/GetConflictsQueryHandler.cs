using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Sync.Application.Contracts;
using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.Application.Queries.GetConflicts;

public sealed class GetConflictsQueryHandler : IQueryHandler<GetConflictsQuery, List<ConflictDto>>
{
    private readonly ISyncBatchReadRepository syncBatches;

    public GetConflictsQueryHandler(ISyncBatchReadRepository syncBatches)
    {
        this.syncBatches = syncBatches;
    }

    public async Task<List<ConflictDto>> Handle(
        GetConflictsQuery query,
        CancellationToken cancellationToken = default)
    {
        var deviceId = query.DeviceId is not null ? DeviceIdentifier.From(query.DeviceId) : null;
        var status = query.Status is not null ? ConflictStatus.From(query.Status) : null;

        return await syncBatches.GetConflictsAsync(deviceId, status, cancellationToken);
    }
}
