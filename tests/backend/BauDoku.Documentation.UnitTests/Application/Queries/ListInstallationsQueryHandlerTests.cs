using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Pagination;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.Dtos;
using BauDoku.Documentation.Application.Queries.ListInstallations;
using BauDoku.Documentation.Domain;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Queries;

public sealed class ListInstallationsQueryHandlerTests
{
    private readonly IInstallationReadRepository readRepository;
    private readonly ListInstallationsQueryHandler handler;

    public ListInstallationsQueryHandlerTests()
    {
        readRepository = Substitute.For<IInstallationReadRepository>();
        handler = new ListInstallationsQueryHandler(readRepository);
    }

    [Fact]
    public async Task Handle_ShouldDelegateToReadRepository()
    {
        var projectId = Guid.NewGuid();
        var items = new List<InstallationListItemDto>
        {
            new(Guid.NewGuid(), projectId, "cable_tray", "in_progress", "b", 48.0, 11.0, "Test", DateTime.UtcNow, 2, 1)
        };
        var expected = new PagedResult<InstallationListItemDto>(items, 1, 1, 20);

        readRepository.ListAsync(
                new InstallationListFilter(ProjectIdentifier.From(projectId)),
                new PaginationParams(1, 20),
                Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await handler.Handle(new ListInstallationsQuery(projectId), CancellationToken.None);

        result.Should().BeSameAs(expected);
        result.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_ShouldPassAllFilterParameters()
    {
        var projectId = Guid.NewGuid();
        var zoneId = Guid.NewGuid();
        var expected = new PagedResult<InstallationListItemDto>([], 0, 2, 10);

        readRepository.ListAsync(
                new InstallationListFilter(
                    ProjectIdentifier.From(projectId),
                    ZoneIdentifier.From(zoneId),
                    InstallationType.From("cable_tray"),
                    InstallationStatus.From("in_progress"),
                    "test"),
                new PaginationParams(2, 10),
                Arg.Any<CancellationToken>())
            .Returns(expected);

        var query = new ListInstallationsQuery(projectId, zoneId, "cable_tray", "in_progress", "test", 2, 10);
        var result = await handler.Handle(query, CancellationToken.None);

        result.Page.Should().Be(2);
        result.PageSize.Should().Be(10);
    }
}
