using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Application.Queries.GetInstallationsInRadius;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Queries;

public sealed class GetInstallationsInRadiusQueryHandlerTests
{
    private readonly IInstallationReadRepository readRepository;
    private readonly GetInstallationsInRadiusQueryHandler handler;

    public GetInstallationsInRadiusQueryHandlerTests()
    {
        readRepository = Substitute.For<IInstallationReadRepository>();
        handler = new GetInstallationsInRadiusQueryHandler(readRepository);
    }

    [Fact]
    public async Task Handle_ShouldDelegateToReadRepository()
    {
        var items = new List<NearbyInstallationDto>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), "cable_tray", "in_progress", "b",
                48.137154, 11.576124, "Test", DateTime.UtcNow, 2, 1, 150.5)
        };
        var expected = new PagedResult<NearbyInstallationDto>(items, 1, 1, 20);

        readRepository.SearchInRadiusAsync(
                48.0, 11.0, 500.0, null, 1, 20, Arg.Any<CancellationToken>())
            .Returns(expected);

        var query = new GetInstallationsInRadiusQuery(48.0, 11.0, 500.0);
        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeSameAs(expected);
        result.Items.Should().ContainSingle();
        result.Items[0].DistanceMeters.Should().Be(150.5);
    }

    [Fact]
    public async Task Handle_WithProjectFilter_ShouldPassProjectId()
    {
        var projectId = Guid.NewGuid();
        var expected = new PagedResult<NearbyInstallationDto>([], 0, 1, 20);

        readRepository.SearchInRadiusAsync(
                48.0, 11.0, 1000.0, projectId, 1, 20, Arg.Any<CancellationToken>())
            .Returns(expected);

        var query = new GetInstallationsInRadiusQuery(48.0, 11.0, 1000.0, projectId);
        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeSameAs(expected);
        await readRepository.Received(1).SearchInRadiusAsync(
            48.0, 11.0, 1000.0, projectId, 1, 20, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldPassPaginationParameters()
    {
        var expected = new PagedResult<NearbyInstallationDto>([], 0, 3, 10);

        readRepository.SearchInRadiusAsync(
                48.0, 11.0, 500.0, null, 3, 10, Arg.Any<CancellationToken>())
            .Returns(expected);

        var query = new GetInstallationsInRadiusQuery(48.0, 11.0, 500.0, Page: 3, PageSize: 10);
        var result = await handler.Handle(query, CancellationToken.None);

        result.Page.Should().Be(3);
        result.PageSize.Should().Be(10);
    }
}
