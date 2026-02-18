using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Commands.RecordMeasurement;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class RecordMeasurementCommandHandlerTests
{
    private readonly IInstallationRepository installations;
    private readonly IUnitOfWork unitOfWork;
    private readonly RecordMeasurementCommandHandler handler;

    public RecordMeasurementCommandHandlerTests()
    {
        installations = Substitute.For<IInstallationRepository>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        handler = new RecordMeasurementCommandHandler(installations, unitOfWork);
    }

    private static Installation CreateValidInstallation() =>
        Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.CableTray,
            GpsPosition.Create(48.137154, 11.576124, null, 3.5, "gps"));

    [Fact]
    public async Task Handle_WithValidCommand_ShouldRecordMeasurementAndReturnId()
    {
        var installation = CreateValidInstallation();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new RecordMeasurementCommand(
            installation.Id.Value, "insulation_resistance", 500.0, "MΩ", 1.0, null, "Notiz");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
        installation.Measurements.Should().ContainSingle();
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInstallationNotFound_ShouldThrow()
    {
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Throws(new KeyNotFoundException());

        var command = new RecordMeasurementCommand(
            Guid.NewGuid(), "insulation_resistance", 500.0, "MΩ", null, null, null);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
