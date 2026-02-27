using AwesomeAssertions;
using BauDoku.Documentation.Application.Queries;
using BauDoku.Documentation.Application.Queries.Handlers;
using BauDoku.Documentation.Domain;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BauDoku.Documentation.UnitTests.Application.Queries;

public sealed class GetInstallationQueryHandlerTests
{
    private readonly IInstallationRepository installations;
    private readonly GetInstallationQueryHandler handler;

    public GetInstallationQueryHandlerTests()
    {
        installations = Substitute.For<IInstallationRepository>();
        handler = new GetInstallationQueryHandler(installations);
    }

    [Fact]
    public async Task Handle_WhenInstallationExists_ShouldReturnDtoWithPhotosAndMeasurements()
    {
        var installation = Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.CableTray,
            GpsPosition.Create(Latitude.From(48.137154), Longitude.From(11.576124), 520.0, HorizontalAccuracy.From(3.5), GpsSource.From("gps")),
            Description.From("Testbeschreibung"));

        installation.AddPhoto(
            PhotoIdentifier.New(), FileName.From("photo.jpg"), BlobUrl.From("https://blob/photo.jpg"), ContentType.From("image/jpeg"), FileSize.From(1024),
            PhotoType.Before, Caption.From("Vorher"));

        installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.InsulationResistance,
            MeasurementValue.Create(500.0, "MΩ", 1.0, null));

        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var result = await handler.Handle(new GetInstallationQuery(installation.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.Type.Should().Be("cable_tray");
        result.Description.Should().Be("Testbeschreibung");
        result.Latitude.Should().Be(48.137154);
        result.Photos.Should().ContainSingle();
        result.Photos[0].FileName.Should().Be("photo.jpg");
        result.Measurements.Should().ContainSingle();
        result.Measurements[0].Type.Should().Be("insulation_resistance");
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldThrow()
    {
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Throws(new KeyNotFoundException());

        var act = () => handler.Handle(new GetInstallationQuery(InstallationIdentifier.New()), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldMapAllGpsFields()
    {
        var installation = Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            ZoneIdentifier.New(),
            InstallationType.JunctionBox,
            GpsPosition.Create(Latitude.From(48.0), Longitude.From(11.0), 500.0, HorizontalAccuracy.From(2.5), GpsSource.From("dgnss"), CorrectionService.From("SAPOS-EPS"), RtkFixStatus.From("fix"), 12, 0.8, 1.5));

        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var result = await handler.Handle(new GetInstallationQuery(installation.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.GpsSource.Should().Be("dgnss");
        result.CorrectionService.Should().Be("SAPOS-EPS");
        result.RtkFixStatus.Should().Be("fix");
        result.SatelliteCount.Should().Be(12);
        result.Hdop.Should().Be(0.8);
        result.CorrectionAge.Should().Be(1.5);
    }
}
