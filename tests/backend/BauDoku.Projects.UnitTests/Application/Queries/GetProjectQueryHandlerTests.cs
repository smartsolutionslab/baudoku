using AwesomeAssertions;
using BauDoku.Projects.Application.Queries;
using BauDoku.Projects.Application.Queries.Handlers;
using BauDoku.Projects.Domain;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BauDoku.Projects.UnitTests.Application.Queries;

public sealed class GetProjectQueryHandlerTests
{
    private readonly IProjectRepository projects;
    private readonly GetProjectQueryHandler handler;

    public GetProjectQueryHandlerTests()
    {
        projects = Substitute.For<IProjectRepository>();
        handler = new GetProjectQueryHandler(projects);
    }

    [Fact]
    public async Task Handle_WhenProjectExists_ShouldReturnDto()
    {
        var project = Project.Create(
            ProjectIdentifier.New(),
            ProjectName.From("Testprojekt"),
            Address.Create(
                Street.From("Musterstraße 1"),
                City.From("Berlin"), ZipCode.From("10115")),
            ClientInfo.Create(
                ClientName.From("Max Mustermann"),
                EmailAddress.From("max@example.com"),
                PhoneNumber.From("+49 30 12345")));

        projects.GetByIdReadOnlyAsync(Arg.Any<ProjectIdentifier>(), Arg.Any<CancellationToken>()).Returns(project);

        var result = await handler.Handle(new GetProjectQuery(project.Id));

        result.Should().NotBeNull();
        result.Name.Should().Be("Testprojekt");
        result.Street.Should().Be("Musterstraße 1");
        result.City.Should().Be("Berlin");
        result.ClientName.Should().Be("Max Mustermann");
        result.ClientEmail.Should().Be("max@example.com");
    }

    [Fact]
    public async Task Handle_WhenProjectNotFound_ShouldThrow()
    {
        projects.GetByIdReadOnlyAsync(Arg.Any<ProjectIdentifier>(), Arg.Any<CancellationToken>())
            .Throws(new KeyNotFoundException());

        var act = () => handler.Handle(new GetProjectQuery(ProjectIdentifier.New()));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenProjectHasZones_ShouldMapZones()
    {
        var project = Project.Create(
            ProjectIdentifier.New(),
            ProjectName.From("Testprojekt"),
            Address.Create(
                Street.From("Musterstraße 1"),
                City.From("Berlin"),
                ZipCode.From("10115")),
            ClientInfo.Create(ClientName.From("Max Mustermann")));

        project.AddZone(
            ZoneIdentifier.New(),
            ZoneName.From("Erdgeschoss"), ZoneType.Floor);

        projects.GetByIdReadOnlyAsync(Arg.Any<ProjectIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(project);

        var result = await handler.Handle(new GetProjectQuery(project.Id));

        result.Should().NotBeNull();
        result.Zones.Should().ContainSingle();
        result.Zones[0].Name.Should().Be("Erdgeschoss");
        result.Zones[0].Type.Should().Be("floor");
    }
}
