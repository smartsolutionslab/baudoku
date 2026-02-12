using AwesomeAssertions;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Application.Queries.GetProject;
using BauDoku.Projects.Domain.Aggregates;
using BauDoku.Projects.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Projects.UnitTests.Application.Queries;

public sealed class GetProjectQueryHandlerTests
{
    private readonly IProjectRepository _repository;
    private readonly GetProjectQueryHandler _handler;

    public GetProjectQueryHandlerTests()
    {
        _repository = Substitute.For<IProjectRepository>();
        _handler = new GetProjectQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenProjectExists_ShouldReturnDto()
    {
        var project = Project.Create(
            ProjectId.New(),
            new ProjectName("Testprojekt"),
            new Address("Musterstraße 1", "Berlin", "10115"),
            new ClientInfo("Max Mustermann", "max@example.com", "+49 30 12345"));

        _repository.GetByIdAsync(Arg.Any<ProjectId>(), Arg.Any<CancellationToken>())
            .Returns(project);

        var result = await _handler.Handle(new GetProjectQuery(project.Id.Value));

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
        _repository.GetByIdAsync(Arg.Any<ProjectId>(), Arg.Any<CancellationToken>())
            .Returns((Project?)null);

        var result = await _handler.Handle(new GetProjectQuery(Guid.NewGuid()));

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenProjectHasZones_ShouldMapZones()
    {
        var project = Project.Create(
            ProjectId.New(),
            new ProjectName("Testprojekt"),
            new Address("Musterstraße 1", "Berlin", "10115"),
            new ClientInfo("Max Mustermann"));

        project.AddZone(ZoneId.New(), new ZoneName("Erdgeschoss"), ZoneType.Floor);

        _repository.GetByIdAsync(Arg.Any<ProjectId>(), Arg.Any<CancellationToken>())
            .Returns(project);

        var result = await _handler.Handle(new GetProjectQuery(project.Id.Value));

        result.Should().NotBeNull();
        result!.Zones.Should().ContainSingle();
        result.Zones[0].Name.Should().Be("Erdgeschoss");
        result.Zones[0].Type.Should().Be("floor");
    }
}
