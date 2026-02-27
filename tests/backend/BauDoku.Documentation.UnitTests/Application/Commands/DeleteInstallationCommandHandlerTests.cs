using AwesomeAssertions;
using BauDoku.Documentation.Application.Commands;
using BauDoku.Documentation.Application.Commands.Handlers;
using BauDoku.Documentation.Domain;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class DeleteInstallationCommandHandlerTests
{
    private readonly IInstallationRepository installations;
    private readonly DeleteInstallationCommandHandler handler;

    public DeleteInstallationCommandHandlerTests()
    {
        installations = Substitute.For<IInstallationRepository>();
        handler = new DeleteInstallationCommandHandler(installations);
    }

    private static Installation CreateValidInstallation() =>
        Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.CableTray,
            GpsPosition.Create(Latitude.From(48.137154), Longitude.From(11.576124), null, HorizontalAccuracy.From(3.5), GpsSource.From("gps")));

    [Fact]
    public async Task Handle_WithExistingInstallation_ShouldDeleteAndSave()
    {
        var installation = CreateValidInstallation();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        await handler.Handle(new DeleteInstallationCommand(installation.Id), CancellationToken.None);

        installation.IsDeleted.Should().BeTrue();
        await installations.Received(1).SaveAsync(Arg.Any<Installation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithExistingInstallation_ShouldRaiseDomainEvent()
    {
        var installation = CreateValidInstallation();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        await handler.Handle(new DeleteInstallationCommand(installation.Id), CancellationToken.None);

        installation.DomainEvents.Should().Contain(e =>
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
