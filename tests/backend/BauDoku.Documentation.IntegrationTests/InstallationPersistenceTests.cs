using AwesomeAssertions;
using BauDoku.Documentation.Domain;
using BauDoku.Documentation.IntegrationTests.Fixtures;
using Marten;

namespace BauDoku.Documentation.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class InstallationPersistenceTests(PostgreSqlFixture fixture)
{
    [Fact]
    public async Task CreateInstallation_ShouldPersistAndRehydrateFromEvents()
    {
        var projectId = ProjectIdentifier.New();
        var zoneId = ZoneIdentifier.New();
        var installation = Installation.Create(
            InstallationIdentifier.New(),
            projectId,
            zoneId,
            InstallationType.CableTray,
            GpsPosition.Create(Latitude.From(48.1351), Longitude.From(11.5820), 520.0, HorizontalAccuracy.From(3.5), GpsSource.From("internal_gps")),
            Description.From("Test installation"),
            CableSpec.Create(CableType.From("NYM-J 5x2.5"), CrossSection.From(25), CableColor.From("grey"), 5),
            Depth.From(600),
            Manufacturer.From("Hager"),
            ModelName.From("VZ312N"),
            SerialNumber.From("SN-12345"));

        await using var session = fixture.Store.LightweightSession();

        var events = installation.DomainEvents.ToArray();
        session.Events.StartStream<Installation>(installation.Id.Value, events);
        await session.SaveChangesAsync();

        var loaded = await PostgreSqlFixture.RehydrateInstallationAsync(session, installation.Id.Value);

        loaded.Should().NotBeNull();
        loaded!.ProjectId.Should().Be(projectId);
        loaded.ZoneId.Should().Be(zoneId);
        loaded.Type.Should().Be(InstallationType.CableTray);
        loaded.Status.Should().Be(InstallationStatus.InProgress);
        loaded.Position.Latitude.Value.Should().Be(48.1351);
        loaded.Position.Longitude.Value.Should().Be(11.5820);
        loaded.Position.Altitude.Should().Be(520.0);
        loaded.Position.HorizontalAccuracy.Value.Should().Be(3.5);
        loaded.Position.Source.Value.Should().Be("internal_gps");
        loaded.Description!.Value.Should().Be("Test installation");
        loaded.CableSpec!.CableType.Value.Should().Be("NYM-J 5x2.5");
        loaded.CableSpec.CrossSection!.Value.Should().Be(25);
        loaded.CableSpec.Color!.Value.Should().Be("grey");
        loaded.CableSpec.ConductorCount.Should().Be(5);
        loaded.Depth!.ValueInMillimeters.Should().Be(600);
        loaded.Manufacturer!.Value.Should().Be("Hager");
        loaded.ModelName!.Value.Should().Be("VZ312N");
        loaded.SerialNumber!.Value.Should().Be("SN-12345");
        loaded.Photos.Should().BeEmpty();
        loaded.Measurements.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateInstallation_WithOptionalFieldsNull_ShouldRehydrate()
    {
        var installation = Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.Grounding,
            GpsPosition.Create(Latitude.From(48.0), Longitude.From(11.0), null, HorizontalAccuracy.From(5.0), GpsSource.From("internal_gps")));

        await using var session = fixture.Store.LightweightSession();

        var events = installation.DomainEvents.ToArray();
        session.Events.StartStream<Installation>(installation.Id.Value, events);
        await session.SaveChangesAsync();

        var loaded = await PostgreSqlFixture.RehydrateInstallationAsync(session, installation.Id.Value);

        loaded.Should().NotBeNull();
        loaded!.ZoneId.Should().BeNull();
        loaded.Description.Should().BeNull();
        loaded.CableSpec.Should().BeNull();
        loaded.Depth.Should().BeNull();
        loaded.Manufacturer.Should().BeNull();
        loaded.ModelName.Should().BeNull();
        loaded.SerialNumber.Should().BeNull();
        loaded.Position.Altitude.Should().BeNull();
    }

    [Fact]
    public async Task CreateInstallation_WithGpsPosition_ShouldRehydrateAllFields()
    {
        var installation = Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.CablePull,
            GpsPosition.Create(
                Latitude.From(48.1351), Longitude.From(11.5820), 520.0, HorizontalAccuracy.From(0.03), GpsSource.From("rtk"),
                CorrectionService.From("sapos_heps"), RtkFixStatus.From("fix"), 14, 0.8, 1.2));

        await using var session = fixture.Store.LightweightSession();

        var events = installation.DomainEvents.ToArray();
        session.Events.StartStream<Installation>(installation.Id.Value, events);
        await session.SaveChangesAsync();

        var loaded = await PostgreSqlFixture.RehydrateInstallationAsync(session, installation.Id.Value);

        loaded.Should().NotBeNull();
        loaded!.Position.CorrectionService!.Value.Should().Be("sapos_heps");
        loaded.Position.RtkFixStatus!.Value.Should().Be("fix");
        loaded.Position.SatelliteCount.Should().Be(14);
        loaded.Position.Hdop.Should().Be(0.8);
        loaded.Position.CorrectionAge.Should().Be(1.2);
    }
}
