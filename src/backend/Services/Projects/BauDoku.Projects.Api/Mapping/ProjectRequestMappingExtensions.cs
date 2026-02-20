using BauDoku.Projects.Application.Commands;
using BauDoku.Projects.Api.Endpoints;
using BauDoku.Projects.Domain;

namespace BauDoku.Projects.Api.Mapping;

public static class ProjectRequestMappingExtensions
{
    public static AddZoneCommand ToCommand(this AddZoneRequest request, Guid projectId) =>
        new(ProjectIdentifier.From(projectId), request.Name, request.Type, request.ParentZoneId);
}
