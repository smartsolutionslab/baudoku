using AwesomeAssertions;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.GetInstallation;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Queries;

public sealed class GetInstallationQueryHandlerTests
{
    private readonly IInstallationRepository _repository;
    private readonly GetInstallationQueryHandler _handler;

    public GetInstallationQueryHandlerTests()
    {
        _repository = Substitute.For<IInstallationRepository>();
        _handler = new GetInstallationQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenInstallationExists_ShouldReturnDtoWithPhotosAndMeasurements()
    {
        var installation = Installation.Create(
            InstallationId.New(),
            Guid.NewGuid(),
            null,
            InstallationType.CableTray,
            new GpsPosition(48.137154, 11.576124, 520.0, 3.5, "gps"),
            new Description("Testbeschreibung"));

        installation.AddPhoto(
            PhotoId.New(), "photo.jpg", "https://blob/photo.jpg", "image/jpeg", 1024,
            PhotoType.Before, new Caption("Vorher"));

        installation.RecordMeasurement(
            MeasurementId.New(),
            MeasurementType.InsulationResistance,
            new MeasurementValue(500.0, "MΩ", 1.0, null));

        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var result = await _handler.Handle(new GetInstallationQuery(installation.Id.Value), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Type.Should().Be("cable_tray");
        result.Description.Should().Be("Testbeschreibung");
        result.Latitude.Should().Be(48.137154);
        result.Photos.Should().ContainSingle();
        result.Photos[0].FileName.Should().Be("photo.jpg");
        result.Measurements.Should().ContainSingle();
        result.Measurements[0].Type.Should().Be("insulation_resistance");
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnNull()
    {
        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns((Installation?)null);

        var result = await _handler.Handle(new GetInstallationQuery(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldMapAllGpsFields()
    {
        var installation = Installation.Create(
            InstallationId.New(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            InstallationType.JunctionBox,
            new GpsPosition(48.0, 11.0, 500.0, 2.5, "dgnss", "SAPOS-EPS", "fix", 12, 0.8, 1.5));

        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var result = await _handler.Handle(new GetInstallationQuery(installation.Id.Value), CancellationToken.None);

        result.Should().NotBeNull();
        result!.GpsSource.Should().Be("dgnss");
        result.CorrectionService.Should().Be("SAPOS-EPS");
        result.RtkFixStatus.Should().Be("fix");
        result.SatelliteCount.Should().Be(12);
        result.Hdop.Should().Be(0.8);
        result.CorrectionAge.Should().Be(1.5);
    }
}
