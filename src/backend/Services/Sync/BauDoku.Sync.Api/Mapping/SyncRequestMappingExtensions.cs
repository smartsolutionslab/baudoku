using BauDoku.Sync.Application.Commands;
using BauDoku.Sync.Api.Endpoints;
using BauDoku.Sync.Domain;

namespace BauDoku.Sync.Api.Mapping;

public static class SyncRequestMappingExtensions
{
    public static ResolveConflictCommand ToCommand(this ResolveConflictRequest request, Guid conflictId) =>
        new(ConflictRecordIdentifier.From(conflictId),
            ConflictResolutionStrategy.From(request.Strategy),
            DeltaPayload.FromNullable(request.MergedPayload));
}
