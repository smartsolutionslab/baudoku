using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Queries.Dtos;
using BauDoku.Projects.Application.Queries;
using BauDoku.Projects.Application.Queries.Handlers;
using NSubstitute;

namespace BauDoku.Projects.UnitTests.Application.Queries;

public sealed class ListProjectsQueryHandlerTests
{
    private readonly IProjectReadRepository readRepository;
    private readonly ListProjectsQueryHandler handler;

    public ListProjectsQueryHandlerTests()
    {
        readRepository = Substitute.For<IProjectReadRepository>();
        handler = new ListProjectsQueryHandler(readRepository);
    }

    [Fact]
    public async Task Handle_ShouldDelegateToReadRepository()
    {
        var items = new List<ProjectListItemDto>
        {
            new(Guid.NewGuid(), "Projekt 1", "draft", "Berlin", "Firma A", DateTime.UtcNow, 3)
        };
        var expected = new PagedResult<ProjectListItemDto>(items, 1, PageNumber.From(1), PageSize.From(20));

        readRepository.ListAsync("test", new PaginationParams(PageNumber.From(1), PageSize.From(20)), Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await handler.Handle(new ListProjectsQuery(SearchTerm.From("test"), PageNumber.From(1), PageSize.From(20)));

        result.Should().BeSameAs(expected);
        result.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_ShouldPassPaginationParameters()
    {
        var expected = new PagedResult<ProjectListItemDto>([], 0, PageNumber.From(2), PageSize.From(10));

        readRepository.ListAsync(null, new PaginationParams(PageNumber.From(2), PageSize.From(10)), Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await handler.Handle(new ListProjectsQuery(null, PageNumber.From(2), PageSize.From(10)));

        result.Page.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(0);
    }
}
