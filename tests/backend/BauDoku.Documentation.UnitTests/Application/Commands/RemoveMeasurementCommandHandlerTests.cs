using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Commands.RemoveMeasurement;
using BauDoku.Documentation.Domain;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class RemoveMeasurementCommandHandlerTests
{
    private readonly IInstallationRepository installations;
    private readonly IUnitOfWork unitOfWork;
    private readonly RemoveMeasurementCommandHandler handler;

    public RemoveMeasurementCommandHandlerTests()
    {
        installations = Substitute.For<IInstallationRepository>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        handler = new RemoveMeasurementCommandHandler(installations, unitOfWork);
    }

    private static Installation CreateInstallationWithMeasurement(out MeasurementIdentifier measurementId)
    {
        var installation = Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.CableTray,
            GpsPosition.Create(Latitude.From(48.137154), Longitude.From(11.576124), null, HorizontalAccuracy.From(3.5), GpsSource.From("gps")));

        measurementId = MeasurementIdentifier.New();
        installation.RecordMeasurement(
            measurementId,
            MeasurementType.InsulationResistance,
            MeasurementValue.Create(500.0, "MΩ"));

        return installation;
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldRemoveMeasurement()
    {
        var installation = CreateInstallationWithMeasurement(out var measurementId);
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new RemoveMeasurementCommand(installation.Id, measurementId);

        await handler.Handle(command, CancellationToken.None);

        installation.Measurements.Should().BeEmpty();
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInstallationNotFound_ShouldThrow()
    {
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Throws(new KeyNotFoundException());

        var command = new RemoveMeasurementCommand(InstallationIdentifier.New(), MeasurementIdentifier.New());

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
