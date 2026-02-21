using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.Sync.Domain;

namespace BauDoku.Sync.Application.Commands;

public sealed record ResolveConflictCommand(
    ConflictRecordIdentifier ConflictId,
    ConflictResolutionStrategy Strategy,
    DeltaPayload? MergedPayload) : ICommand;
