using SmartSolutionsLab.BauDoku.Sync.Application.Commands;
using SmartSolutionsLab.BauDoku.Sync.Api.Endpoints;
using SmartSolutionsLab.BauDoku.Sync.Domain;

namespace SmartSolutionsLab.BauDoku.Sync.Api.Mapping;

public static class SyncRequestMappingExtensions
{
    public static ResolveConflictCommand ToCommand(this ResolveConflictRequest request, Guid conflictId) =>
        new(ConflictRecordIdentifier.From(conflictId),
            ConflictResolutionStrategy.From(request.Strategy),
            DeltaPayload.FromNullable(request.MergedPayload));
}
