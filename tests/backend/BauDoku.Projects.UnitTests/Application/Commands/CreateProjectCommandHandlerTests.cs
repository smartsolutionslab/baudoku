using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Application.Commands.CreateProject;
using BauDoku.Projects.Domain;
using NSubstitute;

namespace BauDoku.Projects.UnitTests.Application.Commands;

public sealed class CreateProjectCommandHandlerTests
{
    private readonly IProjectRepository projects;
    private readonly IUnitOfWork unitOfWork;
    private readonly CreateProjectCommandHandler handler;

    public CreateProjectCommandHandlerTests()
    {
        projects = Substitute.For<IProjectRepository>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        handler = new CreateProjectCommandHandler(projects, unitOfWork);
    }

    private static CreateProjectCommand CreateValidCommand(string name = "Neues Projekt") =>
        new(name, "Musterstraße 1", "Berlin", "10115", "Max Mustermann", "max@example.com", "+49 30 12345");

    [Fact]
    public async Task Handle_WithUniqueName_ShouldCreateProjectAndReturnId()
    {
        projects.ExistsByNameAsync(Arg.Any<ProjectName>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await handler.Handle(CreateValidCommand());

        result.Should().NotBe(Guid.Empty);
        await projects.Received(1).AddAsync(Arg.Any<Project>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateName_ShouldThrowBusinessRuleException()
    {
        projects.ExistsByNameAsync(Arg.Any<ProjectName>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var act = () => handler.Handle(CreateValidCommand());

        await act.Should().ThrowAsync<BusinessRuleException>();
        await projects.DidNotReceive().AddAsync(Arg.Any<Project>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateName_ShouldNotCallSaveChanges()
    {
        projects.ExistsByNameAsync(Arg.Any<ProjectName>(), Arg.Any<CancellationToken>())
            .Returns(true);

        try { await handler.Handle(CreateValidCommand()); } catch { }

        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
