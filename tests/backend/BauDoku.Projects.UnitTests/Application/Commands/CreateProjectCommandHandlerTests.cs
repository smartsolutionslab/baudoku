using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Projects.Application.Commands.CreateProject;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Domain.Aggregates;
using BauDoku.Projects.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Projects.UnitTests.Application.Commands;

public sealed class CreateProjectCommandHandlerTests
{
    private readonly IProjectRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateProjectCommandHandler _handler;

    public CreateProjectCommandHandlerTests()
    {
        _repository = Substitute.For<IProjectRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new CreateProjectCommandHandler(_repository, _unitOfWork);
    }

    private static CreateProjectCommand CreateValidCommand(string name = "Neues Projekt") =>
        new(name, "Musterstraße 1", "Berlin", "10115", "Max Mustermann", "max@example.com", "+49 30 12345");

    [Fact]
    public async Task Handle_WithUniqueName_ShouldCreateProjectAndReturnId()
    {
        _repository.ExistsByNameAsync(Arg.Any<ProjectName>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await _handler.Handle(CreateValidCommand());

        result.Should().NotBe(Guid.Empty);
        await _repository.Received(1).AddAsync(Arg.Any<Project>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateName_ShouldThrowBusinessRuleException()
    {
        _repository.ExistsByNameAsync(Arg.Any<ProjectName>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var act = () => _handler.Handle(CreateValidCommand());

        await act.Should().ThrowAsync<BusinessRuleException>();
        await _repository.DidNotReceive().AddAsync(Arg.Any<Project>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateName_ShouldNotCallSaveChanges()
    {
        _repository.ExistsByNameAsync(Arg.Any<ProjectName>(), Arg.Any<CancellationToken>())
            .Returns(true);

        try { await _handler.Handle(CreateValidCommand()); } catch { }

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
