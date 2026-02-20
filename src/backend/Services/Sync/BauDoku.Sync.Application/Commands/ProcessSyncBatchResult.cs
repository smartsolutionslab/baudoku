using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Domain;

namespace BauDoku.Sync.Application.Commands;

public sealed record ProcessSyncBatchResult(
    SyncBatchIdentifier BatchId,
    int AppliedCount,
    int ConflictCount,
    List<ConflictDto> Conflicts);
