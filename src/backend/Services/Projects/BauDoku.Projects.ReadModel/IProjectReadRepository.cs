using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Domain;

namespace BauDoku.Projects.ReadModel;

public interface IProjectReadRepository : IReadRepository<ProjectDto, ProjectIdentifier>, IPagedReadRepository<ProjectListItemDto, SearchTerm?>
{
}
