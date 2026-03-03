using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Commands;
using SmartSolutionsLab.BauDoku.Sync.Domain;

namespace SmartSolutionsLab.BauDoku.Sync.Application.Commands;

public sealed record ResolveConflictCommand(
    ConflictRecordIdentifier ConflictId,
    ConflictResolutionStrategy Strategy,
    DeltaPayload? MergedPayload) : ICommand;
