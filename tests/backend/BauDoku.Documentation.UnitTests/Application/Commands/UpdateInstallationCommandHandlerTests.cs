using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Application.Commands;
using SmartSolutionsLab.BauDoku.Documentation.Application.Commands.Handlers;
using SmartSolutionsLab.BauDoku.Documentation.Domain;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class UpdateInstallationCommandHandlerTests
{
    private readonly IInstallationRepository installations;
    private readonly UpdateInstallationCommandHandler handler;

    public UpdateInstallationCommandHandlerTests()
    {
        installations = Substitute.For<IInstallationRepository>();
        handler = new UpdateInstallationCommandHandler(installations);
    }

    private static Installation CreateValidInstallation() =>
        Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.CableTray,
            GpsPosition.Create(Latitude.From(48.137154), Longitude.From(11.576124), null, HorizontalAccuracy.From(3.5), GpsSource.From("gps")));

    [Fact]
    public async Task Handle_WithGpsUpdate_ShouldUpdatePosition()
    {
        var installation = CreateValidInstallation();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new UpdateInstallationCommand(
            installation.Id,
            Position: GpsPosition.Create(Latitude.From(52.520008), Longitude.From(13.404954), Altitude.From(34.0), HorizontalAccuracy.From(1.5), GpsSource.From("dgnss")),
            Description: null,
            CableType: null,
            CrossSection: null,
            CableColor: null,
            ConductorCount: null,
            Depth: null,
            Manufacturer: null, ModelName: null, SerialNumber: null);

        await handler.Handle(command, CancellationToken.None);

        installation.Position.Latitude.Value.Should().Be(52.520008);
        installation.Position.Longitude.Value.Should().Be(13.404954);
        await installations.Received(1).SaveAsync(Arg.Any<Installation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDescriptionUpdate_ShouldUpdateDescription()
    {
        var installation = CreateValidInstallation();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new UpdateInstallationCommand(
            installation.Id,
            Position: null,
            Description: Description.From("Neue Beschreibung"),
            CableType: null, CrossSection: null, CableColor: null,
            ConductorCount: null, Depth: null,
            Manufacturer: null, ModelName: null, SerialNumber: null);

        await handler.Handle(command, CancellationToken.None);

        installation.Description.Should().NotBeNull();
        installation.Description!.Value.Should().Be("Neue Beschreibung");
        await installations.Received(1).SaveAsync(Arg.Any<Installation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithCableSpecUpdate_ShouldUpdateCableSpec()
    {
        var installation = CreateValidInstallation();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new UpdateInstallationCommand(
            installation.Id,
            Position: null,
            Description: null,
            CableType: CableType.From("NYM-J"),
            CrossSection: CrossSection.From(2.5m),
            CableColor: CableColor.From("grau"),
            ConductorCount: ConductorCount.From(3),
            Depth: null,
            Manufacturer: null, ModelName: null, SerialNumber: null);

        await handler.Handle(command, CancellationToken.None);

        installation.CableSpec.Should().NotBeNull();
        installation.CableSpec!.CableType.Value.Should().Be("NYM-J");
        installation.CableSpec.CrossSection!.Value.Should().Be(2.5m);
        await installations.Received(1).SaveAsync(Arg.Any<Installation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDepthUpdate_ShouldUpdateDepth()
    {
        var installation = CreateValidInstallation();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new UpdateInstallationCommand(
            installation.Id,
            Position: null,
            Description: null,
            CableType: null, CrossSection: null, CableColor: null,
            ConductorCount: null,
            Depth: Depth.From(600),
            Manufacturer: null, ModelName: null, SerialNumber: null);

        await handler.Handle(command, CancellationToken.None);

        installation.Depth.Should().NotBeNull();
        installation.Depth!.ValueInMillimeters.Should().Be(600);
        await installations.Received(1).SaveAsync(Arg.Any<Installation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInstallationNotFound_ShouldThrow()
    {
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Throws(new KeyNotFoundException());

        var command = new UpdateInstallationCommand(
            InstallationIdentifier.New(),
            Position: null,
            Description: null,
            CableType: null, CrossSection: null, CableColor: null,
            ConductorCount: null, Depth: null,
            Manufacturer: null, ModelName: null, SerialNumber: null);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WithNoUpdates_ShouldStillSave()
    {
        var installation = CreateValidInstallation();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new UpdateInstallationCommand(
            installation.Id,
            Position: null,
            Description: null,
            CableType: null, CrossSection: null, CableColor: null,
            ConductorCount: null, Depth: null,
            Manufacturer: null, ModelName: null, SerialNumber: null);

        await handler.Handle(command, CancellationToken.None);

        await installations.Received(1).SaveAsync(Arg.Any<Installation>(), Arg.Any<CancellationToken>());
    }
}
