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
    private readonly IProjectRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AddZoneCommandHandler _handler;

    public AddZoneCommandHandlerTests()
    {
        _repository = Substitute.For<IProjectRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new AddZoneCommandHandler(_repository, _unitOfWork);
    }

    private static Project CreateValidProject() =>
        Project.Create(
            ProjectId.New(),
            new ProjectName("Testprojekt"),
            new Address("Musterstraße 1", "Berlin", "10115"),
            new ClientInfo("Max Mustermann"));

    [Fact]
    public async Task Handle_WithValidCommand_ShouldAddZoneAndSave()
    {
        var project = CreateValidProject();
        _repository.GetByIdAsync(Arg.Any<ProjectId>(), Arg.Any<CancellationToken>())
            .Returns(project);

        var command = new AddZoneCommand(project.Id.Value, "Erdgeschoss", "floor", null);

        await _handler.Handle(command);

        project.Zones.Should().ContainSingle();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProjectNotFound_ShouldThrow()
    {
        _repository.GetByIdAsync(Arg.Any<ProjectId>(), Arg.Any<CancellationToken>())
            .Returns((Project?)null);

        var command = new AddZoneCommand(Guid.NewGuid(), "Erdgeschoss", "floor", null);

        var act = () => _handler.Handle(command);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_WithParentZoneId_ShouldPassItToAggregate()
    {
        var project = CreateValidProject();
        var parentId = ZoneId.New();
        project.AddZone(parentId, new ZoneName("Gebäude A"), ZoneType.Building);

        _repository.GetByIdAsync(Arg.Any<ProjectId>(), Arg.Any<CancellationToken>())
            .Returns(project);

        var command = new AddZoneCommand(project.Id.Value, "Erdgeschoss", "floor", parentId.Value);

        await _handler.Handle(command);

        project.Zones.Should().HaveCount(2);
        project.Zones[1].ParentZoneId.Should().Be(parentId);
    }
}
