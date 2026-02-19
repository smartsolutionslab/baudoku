using BauDoku.Projects.Application.Queries.Dtos;
using BauDoku.Projects.Domain;

namespace BauDoku.Projects.Application.Mapping;

public static class ProjectMappingExtensions
{
    public static ProjectDto ToDto(this Project project) =>
        new(project.Id.Value,
            project.Name.Value,
            project.Status.Value,
            project.Address.Street.Value,
            project.Address.City.Value,
            project.Address.ZipCode.Value,
            project.Client.Name.Value,
            project.Client.Email?.Value,
            project.Client.Phone?.Value,
            project.CreatedAt,
            project.Zones.Select(z => z.ToDto()).ToList());

    public static ZoneDto ToDto(this Zone zone) =>
        new(zone.Id.Value,
            zone.Name.Value,
            zone.Type.Value,
            zone.ParentZoneIdentifier?.Value);
}
