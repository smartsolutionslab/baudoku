using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Commands.DocumentInstallation;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class DocumentInstallationCommandHandlerTests
{
    private readonly IInstallationRepository installations;
    private readonly IUnitOfWork unitOfWork;
    private readonly DocumentInstallationCommandHandler handler;

    public DocumentInstallationCommandHandlerTests()
    {
        installations = Substitute.For<IInstallationRepository>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        handler = new DocumentInstallationCommandHandler(installations, unitOfWork);
    }

    private static DocumentInstallationCommand CreateValidCommand() =>
        new(Guid.NewGuid(), null, "cable_tray",
            48.137154, 11.576124, 520.0, 3.5, "gps",
            null, null, null, null, null,
            "Testbeschreibung", "NYM", 4m, "grau", 5, 600, "Siemens", "Model X", "SN-123");

    [Fact]
    public async Task Handle_WithFullCommand_ShouldCreateAndReturnId()
    {
        var command = CreateValidCommand();

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
        await installations.Received(1).AddAsync(Arg.Any<Installation>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithGpsData_ShouldCreateInstallationWithPosition()
    {
        var command = CreateValidCommand();
        Installation? captured = null;
        await installations.AddAsync(Arg.Do<Installation>(i => captured = i), Arg.Any<CancellationToken>());

        await handler.Handle(command, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.Position.Latitude.Should().Be(48.137154);
        captured.Position.Longitude.Should().Be(11.576124);
        captured.Position.HorizontalAccuracy.Should().Be(3.5);
    }

    [Fact]
    public async Task Handle_WithOptionalFields_ShouldSetOptionalProperties()
    {
        var command = CreateValidCommand();
        Installation? captured = null;
        await installations.AddAsync(Arg.Do<Installation>(i => captured = i), Arg.Any<CancellationToken>());

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
            Guid.NewGuid(), null, "cable_tray",
            48.137154, 11.576124, null, 3.5, "gps",
            null, null, null, null, null,
            null, null, null, null, null, null, null, null, null);

        Installation? captured = null;
        await installations.AddAsync(Arg.Do<Installation>(i => captured = i), Arg.Any<CancellationToken>());

        await handler.Handle(command, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.Description.Should().BeNull();
        captured.CableSpec.Should().BeNull();
        captured.Depth.Should().BeNull();
        captured.Manufacturer.Should().BeNull();
    }
}
