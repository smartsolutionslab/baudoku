using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Projects.Application.Commands.AddZone;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Domain.Aggregates;
using BauDoku.Projects.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Projects.UnitTests.Application.Commands;

public sealed class AddZoneCommandHandlerTests
{
    private readonly IProjectRepository projects;
    private readonly IUnitOfWork unitOfWork;
    private readonly AddZoneCommandHandler handler;

    public AddZoneCommandHandlerTests()
    {
        projects = Substitute.For<IProjectRepository>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        handler = new AddZoneCommandHandler(projects, unitOfWork);
    }

    private static Project CreateValidProject() =>
        Project.Create(
            ProjectIdentifier.New(),
            ProjectName.From("Testprojekt"),
            Address.Create("Musterstraße 1", "Berlin", "10115"),
            ClientInfo.Create("Max Mustermann"));

    [Fact]
    public async Task Handle_WithValidCommand_ShouldAddZoneAndSave()
    {
        var project = CreateValidProject();
        projects.GetByIdAsync(Arg.Any<ProjectIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(project);

        var command = new AddZoneCommand(project.Id.Value, "Erdgeschoss", "floor", null);

        await handler.Handle(command);

        project.Zones.Should().ContainSingle();
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProjectNotFound_ShouldThrow()
    {
        projects.GetByIdAsync(Arg.Any<ProjectIdentifier>(), Arg.Any<CancellationToken>())
            .Returns((Project?)null);

        var command = new AddZoneCommand(Guid.NewGuid(), "Erdgeschoss", "floor", null);

        var act = () => handler.Handle(command);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_WithParentZoneIdentifier_ShouldPassItToAggregate()
    {
        var project = CreateValidProject();
        var parentId = ZoneIdentifier.New();
        project.AddZone(parentId, ZoneName.From("Gebäude A"), ZoneType.Building);

        projects.GetByIdAsync(Arg.Any<ProjectIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(project);

        var command = new AddZoneCommand(project.Id.Value, "Erdgeschoss", "floor", parentId.Value);

        await handler.Handle(command);

        project.Zones.Should().HaveCount(2);
        project.Zones[1].ParentZoneIdentifier.Should().Be(parentId);
    }
}
