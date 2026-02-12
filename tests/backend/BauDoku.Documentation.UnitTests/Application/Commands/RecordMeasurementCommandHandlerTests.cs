using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Commands.RecordMeasurement;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class RecordMeasurementCommandHandlerTests
{
    private readonly IInstallationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RecordMeasurementCommandHandler _handler;

    public RecordMeasurementCommandHandlerTests()
    {
        _repository = Substitute.For<IInstallationRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new RecordMeasurementCommandHandler(_repository, _unitOfWork);
    }

    private static Installation CreateValidInstallation() =>
        Installation.Create(
            InstallationId.New(),
            Guid.NewGuid(),
            null,
            InstallationType.CableTray,
            new GpsPosition(48.137154, 11.576124, null, 3.5, "gps"));

    [Fact]
    public async Task Handle_WithValidCommand_ShouldRecordMeasurementAndReturnId()
    {
        var installation = CreateValidInstallation();
        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new RecordMeasurementCommand(
            installation.Id.Value, "insulation_resistance", 500.0, "MΩ", 1.0, null, "Notiz");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
        installation.Measurements.Should().ContainSingle();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInstallationNotFound_ShouldThrow()
    {
        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns((Installation?)null);

        var command = new RecordMeasurementCommand(
            Guid.NewGuid(), "insulation_resistance", 500.0, "MΩ", null, null, null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
