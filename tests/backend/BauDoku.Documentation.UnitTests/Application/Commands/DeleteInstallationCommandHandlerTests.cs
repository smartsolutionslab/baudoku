using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Commands.DeleteInstallation;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class DeleteInstallationCommandHandlerTests
{
    private readonly IInstallationRepository installations;
    private readonly IUnitOfWork unitOfWork;
    private readonly DeleteInstallationCommandHandler handler;

    public DeleteInstallationCommandHandlerTests()
    {
        installations = Substitute.For<IInstallationRepository>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        handler = new DeleteInstallationCommandHandler(installations, unitOfWork);
    }

    private static Installation CreateValidInstallation() =>
        Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.CableTray,
            GpsPosition.Create(48.137154, 11.576124, null, 3.5, "gps"));

    [Fact]
    public async Task Handle_WithExistingInstallation_ShouldRemoveAndSave()
    {
        var installation = CreateValidInstallation();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        await handler.Handle(new DeleteInstallationCommand(installation.Id.Value), CancellationToken.None);

        installations.Received(1).Remove(installation);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithExistingInstallation_ShouldRaiseDomainEvent()
    {
        var installation = CreateValidInstallation();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        await handler.Handle(new DeleteInstallationCommand(installation.Id.Value), CancellationToken.None);

        installation.DomainEvents.Should().ContainSingle(e =>
            e.GetType().Name == "InstallationDeleted");
    }

    [Fact]
    public async Task Handle_WhenInstallationNotFound_ShouldThrow()
    {
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns((Installation?)null);

        var act = () => handler.Handle(new DeleteInstallationCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
