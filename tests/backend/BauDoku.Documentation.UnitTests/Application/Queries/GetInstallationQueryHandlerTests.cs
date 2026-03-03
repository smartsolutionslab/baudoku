using AwesomeAssertions;
using BauDoku.Documentation.Application.Queries;
using BauDoku.Documentation.ReadModel;
using BauDoku.Documentation.Application.Queries.Handlers;
using BauDoku.Documentation.Domain;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BauDoku.Documentation.UnitTests.Application.Queries;

public sealed class GetInstallationQueryHandlerTests
{
    private readonly IInstallationReadRepository installations;
    private readonly GetInstallationQueryHandler handler;

    public GetInstallationQueryHandlerTests()
    {
        installations = Substitute.For<IInstallationReadRepository>();
        handler = new GetInstallationQueryHandler(installations);
    }

    [Fact]
    public async Task Handle_WhenInstallationExists_ShouldReturnDtoWithPhotosAndMeasurements()
    {
        var installationId = InstallationIdentifier.New();
        var dto = new InstallationDto(
            installationId.Value,
            Guid.NewGuid(),
            null,
            "cable_tray",
            "in_progress",
            new GpsPositionDto(48.137154, 11.576124, 520.0, 3.5, "gps", null, null, null, null, null),
            "A",
            "Testbeschreibung",
            null, null, null, null, null, null, null, null,
            DateTime.UtcNow,
            null,
            [new PhotoDto(Guid.NewGuid(), installationId.Value, "photo.jpg", "https://blob/photo.jpg", "image/jpeg", 1024, "before", "Vorher", null, null, DateTime.UtcNow)],
            [new MeasurementDto(Guid.NewGuid(), installationId.Value, "insulation_resistance", 500.0, "MΩ", 1.0, null, "pass", null, DateTime.UtcNow)]);

        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(dto);

        var result = await handler.Handle(new GetInstallationQuery(installationId));

        result.Should().NotBeNull();
        result.Type.Should().Be("cable_tray");
        result.Description.Should().Be("Testbeschreibung");
        result.Position.Latitude.Should().Be(48.137154);
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

        var act = () => handler.Handle(new GetInstallationQuery(InstallationIdentifier.New()));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldMapAllGpsFields()
    {
        var installationId = InstallationIdentifier.New();
        var dto = new InstallationDto(
            installationId.Value,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "junction_box",
            "in_progress",
            new GpsPositionDto(48.0, 11.0, 500.0, 2.5, "dgnss", "SAPOS-EPS", "fix", 12, 0.8, 1.5),
            "A",
            null, null, null, null, null, null, null, null, null,
            DateTime.UtcNow,
            null,
            [],
            []);

        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(dto);

        var result = await handler.Handle(new GetInstallationQuery(installationId));

        result.Should().NotBeNull();
        result.Position.GpsSource.Should().Be("dgnss");
        result.Position.CorrectionService.Should().Be("SAPOS-EPS");
        result.Position.RtkFixStatus.Should().Be("fix");
        result.Position.SatelliteCount.Should().Be(12);
        result.Position.Hdop.Should().Be(0.8);
        result.Position.CorrectionAge.Should().Be(1.5);
    }
}
