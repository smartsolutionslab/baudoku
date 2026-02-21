using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Application.Queries;
using BauDoku.Documentation.Application.Queries.Handlers;
using BauDoku.Documentation.Domain;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Queries;

public sealed class GetInstallationsInBoundingBoxQueryHandlerTests
{
    private readonly IInstallationReadRepository readRepository;
    private readonly GetInstallationsInBoundingBoxQueryHandler handler;

    public GetInstallationsInBoundingBoxQueryHandlerTests()
    {
        readRepository = Substitute.For<IInstallationReadRepository>();
        handler = new GetInstallationsInBoundingBoxQueryHandler(readRepository);
    }

    [Fact]
    public async Task Handle_ShouldDelegateToReadRepository()
    {
        var items = new List<InstallationListItemDto>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), "cable_tray", "in_progress", "b",
                48.137154, 11.576124, "Test", DateTime.UtcNow, 2, 1)
        };
        var expected = new PagedResult<InstallationListItemDto>(items, 1, PageNumber.Default, PageSize.Default);

        readRepository.SearchInBoundingBoxAsync(
                new BoundingBox(47.0, 10.0, 49.0, 12.0),
                null,
                new PaginationParams(PageNumber.Default, PageSize.Default),
                Arg.Any<CancellationToken>())
            .Returns(expected);

        var query = new GetInstallationsInBoundingBoxQuery(new BoundingBox(47.0, 10.0, 49.0, 12.0));
        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeSameAs(expected);
        result.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_WithProjectFilter_ShouldPassProjectId()
    {
        var projectId = ProjectIdentifier.New();
        var expected = new PagedResult<InstallationListItemDto>([], 0, PageNumber.Default, PageSize.Default);

        readRepository.SearchInBoundingBoxAsync(
                new BoundingBox(47.0, 10.0, 49.0, 12.0),
                projectId,
                new PaginationParams(PageNumber.Default, PageSize.Default),
                Arg.Any<CancellationToken>())
            .Returns(expected);

        var query = new GetInstallationsInBoundingBoxQuery(new BoundingBox(47.0, 10.0, 49.0, 12.0), projectId);
        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeSameAs(expected);
        await readRepository.Received(1).SearchInBoundingBoxAsync(
            new BoundingBox(47.0, 10.0, 49.0, 12.0),
            projectId,
            new PaginationParams(PageNumber.Default, PageSize.Default),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldPassPaginationParameters()
    {
        var expected = new PagedResult<InstallationListItemDto>([], 0, PageNumber.From(2), PageSize.From(15));

        readRepository.SearchInBoundingBoxAsync(
                new BoundingBox(47.0, 10.0, 49.0, 12.0),
                null,
                new PaginationParams(PageNumber.From(2), PageSize.From(15)),
                Arg.Any<CancellationToken>())
            .Returns(expected);

        var query = new GetInstallationsInBoundingBoxQuery(new BoundingBox(47.0, 10.0, 49.0, 12.0), Page: PageNumber.From(2), PageSize: PageSize.From(15));
        var result = await handler.Handle(query, CancellationToken.None);

        result.Page.Should().Be(2);
        result.PageSize.Should().Be(15);
    }
}
