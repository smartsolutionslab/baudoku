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
        var expected = new PagedResult<NearbyInstallationDto>(items, 1, PageNumber.Default, PageSize.Default);

        readRepository.SearchInRadiusAsync(
                new SearchRadius(48.0, 11.0, 500.0),
                (ProjectIdentifier?)null,
                new PaginationParams(PageNumber.Default, PageSize.Default),
                Arg.Any<CancellationToken>())
            .Returns(expected);

        var query = new GetInstallationsInRadiusQuery(new SearchRadius(48.0, 11.0, 500.0));
        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeSameAs(expected);
        result.Items.Should().ContainSingle();
        result.Items[0].DistanceMeters.Should().Be(150.5);
    }

    [Fact]
    public async Task Handle_WithProjectFilter_ShouldPassProjectId()
    {
        var projectId = ProjectIdentifier.New();
        var expected = new PagedResult<NearbyInstallationDto>([], 0, PageNumber.Default, PageSize.Default);

        readRepository.SearchInRadiusAsync(
                new SearchRadius(48.0, 11.0, 1000.0),
                projectId,
                new PaginationParams(PageNumber.Default, PageSize.Default),
                Arg.Any<CancellationToken>())
            .Returns(expected);

        var query = new GetInstallationsInRadiusQuery(new SearchRadius(48.0, 11.0, 1000.0), projectId);
        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeSameAs(expected);
        await readRepository.Received(1).SearchInRadiusAsync(
            new SearchRadius(48.0, 11.0, 1000.0),
            projectId,
            new PaginationParams(PageNumber.Default, PageSize.Default),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldPassPaginationParameters()
    {
        var expected = new PagedResult<NearbyInstallationDto>([], 0, PageNumber.From(3), PageSize.From(10));

        readRepository.SearchInRadiusAsync(
                new SearchRadius(48.0, 11.0, 500.0),
                (ProjectIdentifier?)null,
                new PaginationParams(PageNumber.From(3), PageSize.From(10)),
                Arg.Any<CancellationToken>())
            .Returns(expected);

        var query = new GetInstallationsInRadiusQuery(new SearchRadius(48.0, 11.0, 500.0), Page: PageNumber.From(3), PageSize: PageSize.From(10));
        var result = await handler.Handle(query, CancellationToken.None);

        result.Page.Should().Be(3);
        result.PageSize.Should().Be(10);
    }
}
