using AwesomeAssertions;
using BauDoku.Documentation.Application.Commands;
using BauDoku.Documentation.Application.Commands.Handlers;
using BauDoku.Documentation.Domain;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class CompleteInstallationCommandHandlerTests
{
    private readonly IInstallationRepository installations;
    private readonly CompleteInstallationCommandHandler handler;

    public CompleteInstallationCommandHandlerTests()
    {
        installations = Substitute.For<IInstallationRepository>();
        handler = new CompleteInstallationCommandHandler(installations);
    }

    private static Installation CreateValidInstallation() =>
        Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.CableTray,
            GpsPosition.Create(Latitude.From(48.137154), Longitude.From(11.576124), null, HorizontalAccuracy.From(3.5), GpsSource.From("gps")));

    [Fact]
    public async Task Handle_WithExistingInstallation_ShouldMarkCompleted()
    {
        var installation = CreateValidInstallation();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        await handler.Handle(new CompleteInstallationCommand(installation.Id), CancellationToken.None);

        installation.Status.Should().Be(InstallationStatus.Completed);
        installation.CompletedAt.Should().NotBeNull();
        await installations.Received(1).SaveAsync(Arg.Any<Installation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInstallationNotFound_ShouldThrow()
    {
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Throws(new KeyNotFoundException());

        var act = () => handler.Handle(new CompleteInstallationCommand(InstallationIdentifier.New()), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenAlreadyCompleted_ShouldThrowBusinessRule()
    {
        var installation = CreateValidInstallation();
        installation.MarkAsCompleted();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var act = () => handler.Handle(new CompleteInstallationCommand(installation.Id), CancellationToken.None);

        await act.Should().ThrowAsync<BauDoku.BuildingBlocks.Domain.BusinessRuleException>();
    }
}
