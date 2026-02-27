using AwesomeAssertions;
using BauDoku.Documentation.Application.Commands;
using BauDoku.Documentation.Application.Commands.Handlers;
using BauDoku.Documentation.Domain;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class DocumentInstallationCommandHandlerTests
{
    private readonly IInstallationRepository installations;
    private readonly DocumentInstallationCommandHandler handler;

    public DocumentInstallationCommandHandlerTests()
    {
        installations = Substitute.For<IInstallationRepository>();
        handler = new DocumentInstallationCommandHandler(installations);
    }

    private static DocumentInstallationCommand CreateValidCommand() =>
        new(ProjectIdentifier.New(), null, InstallationType.CableTray,
            Latitude.From(48.137154), Longitude.From(11.576124), 520.0, HorizontalAccuracy.From(3.5), GpsSource.From("gps"),
            null, null, null, null, null,
            Description.From("Testbeschreibung"), CableType.From("NYM"), CrossSection.From(4m), CableColor.From("grau"), 5, 600,
            Manufacturer.From("Siemens"), ModelName.From("Model X"), SerialNumber.From("SN-123"));

    [Fact]
    public async Task Handle_WithFullCommand_ShouldCreateAndReturnId()
    {
        var command = CreateValidCommand();

        var result = await handler.Handle(command, CancellationToken.None);

        result.Value.Should().NotBe(Guid.Empty);
        await installations.Received(1).SaveAsync(Arg.Any<Installation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithGpsData_ShouldCreateInstallationWithPosition()
    {
        var command = CreateValidCommand();
        Installation? captured = null;
        await installations.SaveAsync(Arg.Do<Installation>(i => captured = i), Arg.Any<CancellationToken>());

        await handler.Handle(command, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.Position.Latitude.Value.Should().Be(48.137154);
        captured.Position.Longitude.Value.Should().Be(11.576124);
        captured.Position.HorizontalAccuracy.Value.Should().Be(3.5);
    }

    [Fact]
    public async Task Handle_WithOptionalFields_ShouldSetOptionalProperties()
    {
        var command = CreateValidCommand();
        Installation? captured = null;
        await installations.SaveAsync(Arg.Do<Installation>(i => captured = i), Arg.Any<CancellationToken>());

        await handler.Handle(command, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.Description!.Value.Should().Be("Testbeschreibung");
        captured.CableSpec!.CableType.Value.Should().Be("NYM");
        captured.Depth!.ValueInMillimeters.Should().Be(600);
        captured.Manufacturer!.Value.Should().Be("Siemens");
    }

    [Fact]
    public async Task Handle_WithNullOptionals_ShouldLeavePropertiesNull()
    {
        var command = new DocumentInstallationCommand(
            ProjectIdentifier.New(), null, InstallationType.CableTray,
            Latitude.From(48.137154), Longitude.From(11.576124), null, HorizontalAccuracy.From(3.5), GpsSource.From("gps"),
            null, null, null, null, null,
            null, null, null, null, null, null, null, null, null);

        Installation? captured = null;
        await installations.SaveAsync(Arg.Do<Installation>(i => captured = i), Arg.Any<CancellationToken>());

        await handler.Handle(command, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.Description.Should().BeNull();
        captured.CableSpec.Should().BeNull();
        captured.Depth.Should().BeNull();
        captured.Manufacturer.Should().BeNull();
    }
}
