using BauDoku.Projects.Application.Commands.AddZone;
using BauDoku.Projects.Api.Endpoints;

namespace BauDoku.Projects.Api.Mapping;

public static class ProjectRequestMappingExtensions
{
    public static AddZoneCommand ToCommand(this AddZoneRequest request, Guid projectId) =>
        new(projectId, request.Name, request.Type, request.ParentZoneId);
}
