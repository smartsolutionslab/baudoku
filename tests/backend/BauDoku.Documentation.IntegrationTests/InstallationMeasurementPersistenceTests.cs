using AwesomeAssertions;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using BauDoku.Documentation.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Documentation.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class InstallationMeasurementPersistenceTests
{
    private readonly PostgreSqlFixture _fixture;

    public InstallationMeasurementPersistenceTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task RecordMeasurement_WithThresholds_ShouldPersistAndLoad()
    {
        var installation = Installation.Create(
            InstallationId.New(),
            Guid.NewGuid(),
            null,
            InstallationType.CableTray,
            new GpsPosition(48.1351, 11.5820, 520.0, 3.5, "internal_gps"));

        var measurementId = MeasurementId.New();
        installation.RecordMeasurement(
            measurementId,
            MeasurementType.Voltage,
            new MeasurementValue(230.0, "V", 220.0, 240.0),
            "Spannungsmessung Phase L1");

        await using (var writeContext = _fixture.CreateContext())
        {
            writeContext.Installations.Add(installation);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = _fixture.CreateContext())
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
            measurement.Value.Unit.Should().Be("V");
            measurement.Value.MinThreshold.Should().Be(220.0);
            measurement.Value.MaxThreshold.Should().Be(240.0);
            measurement.Result.Should().Be(MeasurementResult.Passed);
            measurement.Notes.Should().Be("Spannungsmessung Phase L1");
            measurement.MeasuredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }
    }

    [Fact]
    public async Task RecordMeasurement_WithoutThresholds_ShouldPersist()
    {
        var installation = Installation.Create(
            InstallationId.New(),
            Guid.NewGuid(),
            null,
            InstallationType.JunctionBox,
            new GpsPosition(48.0, 11.0, null, 5.0, "internal_gps"));

        installation.RecordMeasurement(
            MeasurementId.New(),
            MeasurementType.Continuity,
            new MeasurementValue(0.3, "Ohm"));

        await using (var writeContext = _fixture.CreateContext())
        {
            writeContext.Installations.Add(installation);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = _fixture.CreateContext())
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
            InstallationId.New(),
            Guid.NewGuid(),
            null,
            InstallationType.Grounding,
            new GpsPosition(48.0, 11.0, null, 5.0, "internal_gps"));

        installation.RecordMeasurement(
            MeasurementId.New(),
            MeasurementType.InsulationResistance,
            new MeasurementValue(0.5, "MOhm", 1.0, 100.0));

        await using (var writeContext = _fixture.CreateContext())
        {
            writeContext.Installations.Add(installation);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = _fixture.CreateContext())
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
            InstallationId.New(),
            Guid.NewGuid(),
            null,
            InstallationType.Switchgear,
            new GpsPosition(48.0, 11.0, null, 5.0, "internal_gps"));

        installation.RecordMeasurement(MeasurementId.New(), MeasurementType.Voltage, new MeasurementValue(230.0, "V", 220.0, 240.0));
        installation.RecordMeasurement(MeasurementId.New(), MeasurementType.Continuity, new MeasurementValue(0.3, "Ohm"));
        installation.RecordMeasurement(MeasurementId.New(), MeasurementType.RcdTripCurrent, new MeasurementValue(28.0, "mA", 15.0, 30.0));

        await using (var writeContext = _fixture.CreateContext())
        {
            writeContext.Installations.Add(installation);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = _fixture.CreateContext())
        {
            var loaded = await readContext.Installations
                .Include(i => i.Measurements)
                .FirstOrDefaultAsync(i => i.Id == installation.Id);

            loaded.Should().NotBeNull();
            loaded!.Measurements.Should().HaveCount(3);
        }
    }
}
