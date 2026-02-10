using BauDoku.BuildingBlocks.Application.Commands;

namespace BauDoku.Sync.Application.Commands.ResolveConflict;

public sealed record ResolveConflictCommand(
    Guid ConflictId,
    string Strategy,
    string? MergedPayload) : ICommand;
