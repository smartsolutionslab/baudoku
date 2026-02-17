using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Queries.Dtos;
using BauDoku.Projects.Application.Queries.ListProjects;
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
        var expected = new PagedResult<ProjectListItemDto>(items, 1, 1, 20);

        readRepository.ListAsync("test", new PaginationParams(1, 20), Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await handler.Handle(new ListProjectsQuery("test", 1, 20));

        result.Should().BeSameAs(expected);
        result.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_ShouldPassPaginationParameters()
    {
        var expected = new PagedResult<ProjectListItemDto>([], 0, 2, 10);

        readRepository.ListAsync(null, new PaginationParams(2, 10), Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await handler.Handle(new ListProjectsQuery(null, 2, 10));

        result.Page.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(0);
    }
}
