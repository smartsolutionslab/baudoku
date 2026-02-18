using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Projects.Application.Commands.DeleteProject;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Domain.Aggregates;
using BauDoku.Projects.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BauDoku.Projects.UnitTests.Application.Commands;

public sealed class DeleteProjectCommandHandlerTests
{
    private readonly IProjectRepository projects;
    private readonly IUnitOfWork unitOfWork;
    private readonly DeleteProjectCommandHandler handler;

    public DeleteProjectCommandHandlerTests()
    {
        projects = Substitute.For<IProjectRepository>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        handler = new DeleteProjectCommandHandler(projects, unitOfWork);
    }

    private static Project CreateTestProject() =>
        Project.Create(
            ProjectIdentifier.New(),
            ProjectName.From("Test Projekt"),
            Address.Create(Street.From("Musterstraße 1"), City.From("Berlin"), ZipCode.From("10115")),
            ClientInfo.Create("Max Mustermann", "max@example.com", "+49 30 12345"));

    [Fact]
    public async Task Handle_WithExistingProject_ShouldRemoveAndSave()
    {
        var project = CreateTestProject();
        projects.GetByIdAsync(Arg.Any<ProjectIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(project);

        var command = new DeleteProjectCommand(project.Id.Value);

        await handler.Handle(command);

        projects.Received(1).Remove(project);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithExistingProject_ShouldRaiseDomainEvent()
    {
        var project = CreateTestProject();
        projects.GetByIdAsync(Arg.Any<ProjectIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(project);

        var command = new DeleteProjectCommand(project.Id.Value);

        await handler.Handle(command);

        project.DomainEvents.Should().ContainSingle(e =>
            e.GetType().Name == "ProjectDeleted");
    }

    [Fact]
    public async Task Handle_WhenProjectNotFound_ShouldThrow()
    {
        projects.GetByIdAsync(Arg.Any<ProjectIdentifier>(), Arg.Any<CancellationToken>())
            .Throws(new KeyNotFoundException());

        var command = new DeleteProjectCommand(Guid.NewGuid());

        var act = () => handler.Handle(command);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenProjectNotFound_ShouldNotCallSaveChanges()
    {
        projects.GetByIdAsync(Arg.Any<ProjectIdentifier>(), Arg.Any<CancellationToken>())
            .Throws(new KeyNotFoundException());

        try { await handler.Handle(new DeleteProjectCommand(Guid.NewGuid())); } catch { }

        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
