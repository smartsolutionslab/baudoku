using SmartSolutionsLab.BauDoku.Projects.Application.Commands;
using SmartSolutionsLab.BauDoku.Projects.Api.Endpoints;
using SmartSolutionsLab.BauDoku.Projects.Domain;

namespace SmartSolutionsLab.BauDoku.Projects.Api.Mapping;

public static class ProjectRequestMappingExtensions
{
    public static AddZoneCommand ToCommand(this AddZoneRequest request, Guid projectId) =>
        new(ProjectIdentifier.From(projectId), request.Name, request.Type, request.ParentZoneId);
}
