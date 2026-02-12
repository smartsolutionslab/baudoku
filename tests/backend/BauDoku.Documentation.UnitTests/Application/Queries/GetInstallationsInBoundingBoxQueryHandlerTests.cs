using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Application.Queries.GetInstallationsInBoundingBox;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Queries;

public sealed class GetInstallationsInBoundingBoxQueryHandlerTests
{
    private readonly IInstallationReadRepository _readRepository;
    private readonly GetInstallationsInBoundingBoxQueryHandler _handler;

    public GetInstallationsInBoundingBoxQueryHandlerTests()
    {
        _readRepository = Substitute.For<IInstallationReadRepository>();
        _handler = new GetInstallationsInBoundingBoxQueryHandler(_readRepository);
    }

    [Fact]
    public async Task Handle_ShouldDelegateToReadRepository()
    {
        var items = new List<InstallationListItemDto>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), "cable_tray", "in_progress", "b",
                48.137154, 11.576124, "Test", DateTime.UtcNow, 2, 1)
        };
        var expected = new PagedResult<InstallationListItemDto>(items, 1, 1, 20);

        _readRepository.SearchInBoundingBoxAsync(
                47.0, 10.0, 49.0, 12.0, null, 1, 20, Arg.Any<CancellationToken>())
            .Returns(expected);

        var query = new GetInstallationsInBoundingBoxQuery(47.0, 10.0, 49.0, 12.0);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeSameAs(expected);
        result.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_WithProjectFilter_ShouldPassProjectId()
    {
        var projectId = Guid.NewGuid();
        var expected = new PagedResult<InstallationListItemDto>([], 0, 1, 20);

        _readRepository.SearchInBoundingBoxAsync(
                47.0, 10.0, 49.0, 12.0, projectId, 1, 20, Arg.Any<CancellationToken>())
            .Returns(expected);

        var query = new GetInstallationsInBoundingBoxQuery(47.0, 10.0, 49.0, 12.0, projectId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeSameAs(expected);
        await _readRepository.Received(1).SearchInBoundingBoxAsync(
            47.0, 10.0, 49.0, 12.0, projectId, 1, 20, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldPassPaginationParameters()
    {
        var expected = new PagedResult<InstallationListItemDto>([], 0, 2, 15);

        _readRepository.SearchInBoundingBoxAsync(
                47.0, 10.0, 49.0, 12.0, null, 2, 15, Arg.Any<CancellationToken>())
            .Returns(expected);

        var query = new GetInstallationsInBoundingBoxQuery(47.0, 10.0, 49.0, 12.0, Page: 2, PageSize: 15);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Page.Should().Be(2);
        result.PageSize.Should().Be(15);
    }
}
