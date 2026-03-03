using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Application.Pagination;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.Documentation.ReadModel;
using SmartSolutionsLab.BauDoku.Documentation.Application.Queries;
using SmartSolutionsLab.BauDoku.Documentation.Application.Queries.Handlers;
using SmartSolutionsLab.BauDoku.Documentation.Domain;
using NSubstitute;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Application.Queries;

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
        var projectId = ProjectIdentifier.New();
        var items = new List<InstallationListItemDto>
        {
            new(Guid.NewGuid(), projectId.Value, "cable_tray", "in_progress", "b", 48.0, 11.0, "Test", DateTime.UtcNow, 2, 1)
        };
        var expected = new PagedResult<InstallationListItemDto>(items, 1, PageNumber.Default, PageSize.Default);

        readRepository.ListAsync(
                new InstallationListFilter(projectId),
                new PaginationParams(PageNumber.Default, PageSize.Default),
                Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await handler.Handle(new ListInstallationsQuery(PageNumber.Default, PageSize.Default, projectId), CancellationToken.None);

        result.Should().BeSameAs(expected);
        result.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_ShouldPassAllFilterParameters()
    {
        var projectId = ProjectIdentifier.New();
        var zoneId = ZoneIdentifier.New();
        var expected = new PagedResult<InstallationListItemDto>([], 0, PageNumber.From(2), PageSize.From(10));

        readRepository.ListAsync(
                new InstallationListFilter(
                    projectId,
                    zoneId,
                    InstallationType.From("cable_tray"),
                    InstallationStatus.From("in_progress"),
                    SearchTerm.From("test")),
                new PaginationParams(PageNumber.From(2), PageSize.From(10)),
                Arg.Any<CancellationToken>())
            .Returns(expected);

        var query = new ListInstallationsQuery(
            PageNumber.From(2), PageSize.From(10),
            projectId, zoneId,
            InstallationType.From("cable_tray"),
            InstallationStatus.From("in_progress"),
            SearchTerm.From("test"));
        var result = await handler.Handle(query, CancellationToken.None);

        result.Page.Should().Be(2);
        result.PageSize.Should().Be(10);
    }
}
