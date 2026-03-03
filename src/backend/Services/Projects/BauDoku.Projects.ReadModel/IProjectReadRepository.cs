using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Pagination;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.Projects.Domain;

namespace SmartSolutionsLab.BauDoku.Projects.ReadModel;

public interface IProjectReadRepository : IReadRepository<ProjectDto, ProjectIdentifier, ProjectListItemDto, SearchTerm?>
{
}
