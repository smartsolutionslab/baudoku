using AwesomeAssertions;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Queries.GetProject;
using BauDoku.Projects.Domain.Aggregates;
using BauDoku.Projects.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Projects.UnitTests.Application.Queries;

public sealed class GetProjectQueryHandlerTests
{
    private readonly IProjectRepository repository;
    private readonly GetProjectQueryHandler handler;

    public GetProjectQueryHandlerTests()
    {
        repository = Substitute.For<IProjectRepository>();
        handler = new GetProjectQueryHandler(repository);
    }

    [Fact]
    public async Task Handle_WhenProjectExists_ShouldReturnDto()
    {
        var project = Project.Create(
            ProjectIdentifier.New(),
            ProjectName.From("Testprojekt"),
            Address.Create("Musterstraße 1", "Berlin", "10115"),
            ClientInfo.Create("Max Mustermann", "max@example.com", "+49 30 12345"));

        repository.GetByIdAsync(Arg.Any<ProjectIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(project);

        var result = await handler.Handle(new GetProjectQuery(project.Id.Value));

        result.Should().NotBeNull();
        result!.Name.Should().Be("Testprojekt");
        result.Street.Should().Be("Musterstraße 1");
        result.City.Should().Be("Berlin");
        result.ClientName.Should().Be("Max Mustermann");
        result.ClientEmail.Should().Be("max@example.com");
    }

    [Fact]
    public async Task Handle_WhenProjectNotFound_ShouldReturnNull()
    {
        repository.GetByIdAsync(Arg.Any<ProjectIdentifier>(), Arg.Any<CancellationToken>())
            .Returns((Project?)null);

        var result = await handler.Handle(new GetProjectQuery(Guid.NewGuid()));

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenProjectHasZones_ShouldMapZones()
    {
        var project = Project.Create(
            ProjectIdentifier.New(),
            ProjectName.From("Testprojekt"),
            Address.Create("Musterstraße 1", "Berlin", "10115"),
            ClientInfo.Create("Max Mustermann"));

        project.AddZone(ZoneIdentifier.New(), ZoneName.From("Erdgeschoss"), ZoneType.Floor);

        repository.GetByIdAsync(Arg.Any<ProjectIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(project);

        var result = await handler.Handle(new GetProjectQuery(project.Id.Value));

        result.Should().NotBeNull();
        result!.Zones.Should().ContainSingle();
        result.Zones[0].Name.Should().Be("Erdgeschoss");
        result.Zones[0].Type.Should().Be("floor");
    }
}
