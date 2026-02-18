using AwesomeAssertions;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using BauDoku.Documentation.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Documentation.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class InstallationMeasurementPersistenceTests(PostgreSqlFixture fixture)
{
    [Fact]
    public async Task RecordMeasurement_WithThresholds_ShouldPersistAndLoad()
    {
        var installation = Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.CableTray,
            GpsPosition.Create(Latitude.From(48.1351), Longitude.From(11.5820), 520.0, HorizontalAccuracy.From(3.5), GpsSource.From("internal_gps")));

        var measurementId = MeasurementIdentifier.New();
        installation.RecordMeasurement(
            measurementId,
            MeasurementType.Voltage,
            MeasurementValue.Create(230.0, "V", 220.0, 240.0),
            Notes.From("Spannungsmessung Phase L1"));

        await using (var writeContext = fixture.CreateContext())
        {
            writeContext.Installations.Add(installation);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = fixture.CreateContext())
        {
            var loaded = await readContext.Installations
                .Include(i => i.Measurements)
                .FirstOrDefaultAsync(i => i.Id == installation.Id);

            loaded.Should().NotBeNull();
            loaded!.Measurements.Should().ContainSingle();

            var measurement = loaded.Measurements[0];
            measurement.Id.Should().Be(measurementId);
            measurement.Type.Should().Be(MeasurementType.Voltage);
            measurement.Value.Value.Should().Be(230.0);
            measurement.Value.Unit.Value.Should().Be("V");
            measurement.Value.MinThreshold.Should().Be(220.0);
            measurement.Value.MaxThreshold.Should().Be(240.0);
            measurement.Result.Should().Be(MeasurementResult.Passed);
            measurement.Notes!.Value.Should().Be("Spannungsmessung Phase L1");
            measurement.MeasuredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }
    }

    [Fact]
    public async Task RecordMeasurement_WithoutThresholds_ShouldPersist()
    {
        var installation = Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.JunctionBox,
            GpsPosition.Create(Latitude.From(48.0), Longitude.From(11.0), null, HorizontalAccuracy.From(5.0), GpsSource.From("internal_gps")));

        installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.Continuity,
            MeasurementValue.Create(0.3, "Ohm"));

        await using (var writeContext = fixture.CreateContext())
        {
            writeContext.Installations.Add(installation);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = fixture.CreateContext())
        {
            var loaded = await readContext.Installations
                .Include(i => i.Measurements)
                .FirstOrDefaultAsync(i => i.Id == installation.Id);

            loaded.Should().NotBeNull();
            var measurement = loaded!.Measurements[0];
            measurement.Value.MinThreshold.Should().BeNull();
            measurement.Value.MaxThreshold.Should().BeNull();
            measurement.Result.Should().Be(MeasurementResult.Passed);
            measurement.Notes.Should().BeNull();

        }
    }

    [Fact]
    public async Task RecordMeasurement_WithFailedResult_ShouldPersist()
    {
        var installation = Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.Grounding,
            GpsPosition.Create(Latitude.From(48.0), Longitude.From(11.0), null, HorizontalAccuracy.From(5.0), GpsSource.From("internal_gps")));

        installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.InsulationResistance,
            MeasurementValue.Create(0.5, "MOhm", 1.0, 100.0));

        await using (var writeContext = fixture.CreateContext())
        {
            writeContext.Installations.Add(installation);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = fixture.CreateContext())
        {
            var loaded = await readContext.Installations
                .Include(i => i.Measurements)
                .FirstOrDefaultAsync(i => i.Id == installation.Id);

            loaded.Should().NotBeNull();
            loaded!.Measurements[0].Result.Should().Be(MeasurementResult.Failed);
        }
    }

    [Fact]
    public async Task RecordMultipleMeasurements_ShouldPersistAll()
    {
        var installation = Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.Switchgear,
            GpsPosition.Create(Latitude.From(48.0), Longitude.From(11.0), null, HorizontalAccuracy.From(5.0), GpsSource.From("internal_gps")));

        installation.RecordMeasurement(MeasurementIdentifier.New(), MeasurementType.Voltage, MeasurementValue.Create(230.0, "V", 220.0, 240.0));
        installation.RecordMeasurement(MeasurementIdentifier.New(), MeasurementType.Continuity, MeasurementValue.Create(0.3, "Ohm"));
        installation.RecordMeasurement(MeasurementIdentifier.New(), MeasurementType.RcdTripCurrent, MeasurementValue.Create(28.0, "mA", 15.0, 30.0));

        await using (var writeContext = fixture.CreateContext())
        {
            writeContext.Installations.Add(installation);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = fixture.CreateContext())
        {
            var loaded = await readContext.Installations
                .Include(i => i.Measurements)
                .FirstOrDefaultAsync(i => i.Id == installation.Id);

            loaded.Should().NotBeNull();
            loaded!.Measurements.Should().HaveCount(3);
        }
    }
}
