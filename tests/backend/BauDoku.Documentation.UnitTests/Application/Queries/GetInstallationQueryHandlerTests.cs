using AwesomeAssertions;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Application.Queries.GetInstallation;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Queries;

public sealed class GetInstallationQueryHandlerTests
{
    private readonly IInstallationRepository repository;
    private readonly GetInstallationQueryHandler handler;

    public GetInstallationQueryHandlerTests()
    {
        repository = Substitute.For<IInstallationRepository>();
        handler = new GetInstallationQueryHandler(repository);
    }

    [Fact]
    public async Task Handle_WhenInstallationExists_ShouldReturnDtoWithPhotosAndMeasurements()
    {
        var installation = Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.CableTray,
            GpsPosition.Create(48.137154, 11.576124, 520.0, 3.5, "gps"),
            Description.From("Testbeschreibung"));

        installation.AddPhoto(
            PhotoIdentifier.New(), "photo.jpg", "https://blob/photo.jpg", "image/jpeg", 1024,
            PhotoType.Before, Caption.From("Vorher"));

        installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.InsulationResistance,
            MeasurementValue.Create(500.0, "MΩ", 1.0, null));

        repository.GetByIdReadOnlyAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var result = await handler.Handle(new GetInstallationQuery(installation.Id.Value), CancellationToken.None);

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
        repository.GetByIdReadOnlyAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns((Installation?)null);

        var result = await handler.Handle(new GetInstallationQuery(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldMapAllGpsFields()
    {
        var installation = Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            ZoneIdentifier.New(),
            InstallationType.JunctionBox,
            GpsPosition.Create(48.0, 11.0, 500.0, 2.5, "dgnss", "SAPOS-EPS", "fix", 12, 0.8, 1.5));

        repository.GetByIdReadOnlyAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var result = await handler.Handle(new GetInstallationQuery(installation.Id.Value), CancellationToken.None);

        result.Should().NotBeNull();
        result!.GpsSource.Should().Be("dgnss");
        result.CorrectionService.Should().Be("SAPOS-EPS");
        result.RtkFixStatus.Should().Be("fix");
        result.SatelliteCount.Should().Be(12);
        result.Hdop.Should().Be(0.8);
        result.CorrectionAge.Should().Be(1.5);
    }
}
