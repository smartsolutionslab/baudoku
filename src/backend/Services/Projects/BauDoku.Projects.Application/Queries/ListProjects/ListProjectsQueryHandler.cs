using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Queries.Dtos;

namespace BauDoku.Projects.Application.Queries.ListProjects;

public sealed class ListProjectsQueryHandler : IQueryHandler<ListProjectsQuery, PagedResult<ProjectListItemDto>>
{
    private readonly IProjectReadRepository readRepository;

    public ListProjectsQueryHandler(IProjectReadRepository readRepository)
    {
        this.readRepository = readRepository;
    }

    public async Task<PagedResult<ProjectListItemDto>> Handle(ListProjectsQuery query, CancellationToken cancellationToken = default)
    {
        return await readRepository.ListAsync(query.Search, query.Page, query.PageSize, cancellationToken);
    }
}
