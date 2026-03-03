using SmartSolutionsLab.BauDoku.Sync.ReadModel;
using SmartSolutionsLab.BauDoku.Sync.Domain;

namespace SmartSolutionsLab.BauDoku.Sync.Application.Commands;

public sealed record ProcessSyncBatchResult(
    SyncBatchIdentifier BatchId,
    int AppliedCount,
    int ConflictCount,
    List<ConflictDto> Conflicts);
