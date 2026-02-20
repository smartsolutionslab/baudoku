using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Commands;
using BauDoku.Documentation.Application.Commands.Handlers;
using BauDoku.Documentation.Domain;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            GpsPosition.Create(Latitude.From(48.137154), Longitude.From(11.576124), null, HorizontalAccuracy.From(3.5), GpsSource.From("gps")));

    [Fact]
    public async Task Handle_WithExistingInstallation_ShouldRemoveAndSave()
    {
        var installation = CreateValidInstallation();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        await handler.Handle(new DeleteInstallationCommand(installation.Id), CancellationToken.None);

        installations.Received(1).Remove(installation);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithExistingInstallation_ShouldRaiseDomainEvent()
    {
        var installation = CreateValidInstallation();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        await handler.Handle(new DeleteInstallationCommand(installation.Id), CancellationToken.None);

        installation.DomainEvents.Should().ContainSingle(e =>
            e.GetType().Name == "InstallationDeleted");
    }

    [Fact]
    public async Task Handle_WhenInstallationNotFound_ShouldThrow()
    {
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Throws(new KeyNotFoundException());

        var act = () => handler.Handle(new DeleteInstallationCommand(InstallationIdentifier.New()), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
