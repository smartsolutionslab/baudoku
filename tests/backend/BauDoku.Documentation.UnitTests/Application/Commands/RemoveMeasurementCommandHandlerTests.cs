using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Commands.RemoveMeasurement;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class RemoveMeasurementCommandHandlerTests
{
    private readonly IInstallationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RemoveMeasurementCommandHandler _handler;

    public RemoveMeasurementCommandHandlerTests()
    {
        _repository = Substitute.For<IInstallationRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new RemoveMeasurementCommandHandler(_repository, _unitOfWork);
    }

    private static Installation CreateInstallationWithMeasurement(out MeasurementId measurementId)
    {
        var installation = Installation.Create(
            InstallationId.New(),
            Guid.NewGuid(),
            null,
            InstallationType.CableTray,
            new GpsPosition(48.137154, 11.576124, null, 3.5, "gps"));

        measurementId = MeasurementId.New();
        installation.RecordMeasurement(
            measurementId,
            MeasurementType.InsulationResistance,
            new MeasurementValue(500.0, "MΩ"));

        return installation;
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldRemoveMeasurement()
    {
        var installation = CreateInstallationWithMeasurement(out var measurementId);
        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new RemoveMeasurementCommand(installation.Id.Value, measurementId.Value);

        await _handler.Handle(command, CancellationToken.None);

        installation.Measurements.Should().BeEmpty();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInstallationNotFound_ShouldThrow()
    {
        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns((Installation?)null);

        var command = new RemoveMeasurementCommand(Guid.NewGuid(), Guid.NewGuid());

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
