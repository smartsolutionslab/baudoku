using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Queries.Dtos;
using BauDoku.Projects.Application.Queries.ListProjects;
using NSubstitute;

namespace BauDoku.Projects.UnitTests.Application.Queries;

public sealed class ListProjectsQueryHandlerTests
{
    private readonly IProjectReadRepository _readRepository;
    private readonly ListProjectsQueryHandler _handler;

    public ListProjectsQueryHandlerTests()
    {
        _readRepository = Substitute.For<IProjectReadRepository>();
        _handler = new ListProjectsQueryHandler(_readRepository);
    }

    [Fact]
    public async Task Handle_ShouldDelegateToReadRepository()
    {
        var items = new List<ProjectListItemDto>
        {
            new(Guid.NewGuid(), "Projekt 1", "draft", "Berlin", "Firma A", DateTime.UtcNow, 3)
        };
        var expected = new PagedResult<ProjectListItemDto>(items, 1, 1, 20);

        _readRepository.ListAsync("test", 1, 20, Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await _handler.Handle(new ListProjectsQuery("test", 1, 20));

        result.Should().BeSameAs(expected);
        result.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_ShouldPassPaginationParameters()
    {
        var expected = new PagedResult<ProjectListItemDto>([], 0, 2, 10);

        _readRepository.ListAsync(null, 2, 10, Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await _handler.Handle(new ListProjectsQuery(null, 2, 10));

        result.Page.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(0);
    }
}
