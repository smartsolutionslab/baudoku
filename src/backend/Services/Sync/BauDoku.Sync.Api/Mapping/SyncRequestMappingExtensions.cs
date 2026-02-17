using BauDoku.Sync.Application.Commands.ResolveConflict;
using BauDoku.Sync.Api.Endpoints;

namespace BauDoku.Sync.Api.Mapping;

public static class SyncRequestMappingExtensions
{
    public static ResolveConflictCommand ToCommand(this ResolveConflictRequest request, Guid conflictId) =>
        new(conflictId, request.Strategy, request.MergedPayload);
}
