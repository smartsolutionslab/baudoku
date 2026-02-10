using AwesomeAssertions;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.Events;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.Aggregates;

public sealed class InstallationMeasurementTests
{
    private static Installation CreateValidInstallation()
    {
        return Installation.Create(
            InstallationId.New(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            InstallationType.CableTray,
            new GpsPosition(48.1351, 11.5820, 520.0, 3.5, "internal_gps"),
            new Description("Kabeltrasse im Erdgeschoss"));
    }

    [Fact]
    public void RecordMeasurement_ShouldAddMeasurementToCollection()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();
        var measurementId = MeasurementId.New();

        installation.RecordMeasurement(
            measurementId,
            MeasurementType.Voltage,
            new MeasurementValue(230.0, "V"),
            "Spannungsmessung");

        installation.Measurements.Should().ContainSingle();
        var measurement = installation.Measurements[0];
        measurement.Id.Should().Be(measurementId);
        measurement.Type.Should().Be(MeasurementType.Voltage);
        measurement.Value.Value.Should().Be(230.0);
        measurement.Value.Unit.Should().Be("V");
        measurement.Notes.Should().Be("Spannungsmessung");
    }

    [Fact]
    public void RecordMeasurement_ShouldRaiseMeasurementRecordedEvent()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();
        var measurementId = MeasurementId.New();

        installation.RecordMeasurement(
            measurementId,
            MeasurementType.Voltage,
            new MeasurementValue(230.0, "V"));

        installation.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<MeasurementRecorded>()
            .Which.MeasurementId.Should().Be(measurementId);
    }

    [Fact]
    public void RecordMeasurement_WithThresholdsInRange_ShouldEvaluateAsPassed()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();

        installation.RecordMeasurement(
            MeasurementId.New(),
            MeasurementType.Voltage,
            new MeasurementValue(230.0, "V", 220.0, 240.0));

        var measurement = installation.Measurements[0];
        measurement.Result.Should().Be(MeasurementResult.Passed);
    }

    [Fact]
    public void RecordMeasurement_WithValueBelowMinThreshold_ShouldEvaluateAsFailed()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();

        installation.RecordMeasurement(
            MeasurementId.New(),
            MeasurementType.Voltage,
            new MeasurementValue(200.0, "V", 220.0, 240.0));

        var measurement = installation.Measurements[0];
        measurement.Result.Should().Be(MeasurementResult.Failed);
    }

    [Fact]
    public void RecordMeasurement_WithValueAboveMaxThreshold_ShouldEvaluateAsFailed()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();

        installation.RecordMeasurement(
            MeasurementId.New(),
            MeasurementType.Voltage,
            new MeasurementValue(250.0, "V", 220.0, 240.0));

        var measurement = installation.Measurements[0];
        measurement.Result.Should().Be(MeasurementResult.Failed);
    }

    [Fact]
    public void RecordMeasurement_WithoutThresholds_ShouldEvaluateAsPassed()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();

        installation.RecordMeasurement(
            MeasurementId.New(),
            MeasurementType.Voltage,
            new MeasurementValue(230.0, "V"));

        var measurement = installation.Measurements[0];
        measurement.Result.Should().Be(MeasurementResult.Passed);
    }

    [Fact]
    public void RecordMeasurement_WithValueAtMinThreshold_ShouldEvaluateAsPassed()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();

        installation.RecordMeasurement(
            MeasurementId.New(),
            MeasurementType.Voltage,
            new MeasurementValue(220.0, "V", 220.0, 240.0));

        var measurement = installation.Measurements[0];
        measurement.Result.Should().Be(MeasurementResult.Passed);
    }

    [Fact]
    public void RecordMeasurement_WithValueAtMaxThreshold_ShouldEvaluateAsPassed()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();

        installation.RecordMeasurement(
            MeasurementId.New(),
            MeasurementType.Voltage,
            new MeasurementValue(240.0, "V", 220.0, 240.0));

        var measurement = installation.Measurements[0];
        measurement.Result.Should().Be(MeasurementResult.Passed);
    }

    [Fact]
    public void RecordMeasurement_WithOnlyMinThresholdAndValueAbove_ShouldPass()
    {
        var installation = CreateValidInstallation();

        installation.RecordMeasurement(
            MeasurementId.New(),
            MeasurementType.InsulationResistance,
            new MeasurementValue(1.5, "MOhm", minThreshold: 1.0));

        installation.Measurements[0].Result.Should().Be(MeasurementResult.Passed);
    }

    [Fact]
    public void RecordMeasurement_WithOnlyMinThresholdAndValueBelow_ShouldFail()
    {
        var installation = CreateValidInstallation();

        installation.RecordMeasurement(
            MeasurementId.New(),
            MeasurementType.InsulationResistance,
            new MeasurementValue(0.5, "MOhm", minThreshold: 1.0));

        installation.Measurements[0].Result.Should().Be(MeasurementResult.Failed);
    }

    [Fact]
    public void RecordMeasurement_WithOptionalNotesNull_ShouldSucceed()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();

        installation.RecordMeasurement(
            MeasurementId.New(),
            MeasurementType.Continuity,
            new MeasurementValue(0.5, "Ohm"));

        installation.Measurements[0].Notes.Should().BeNull();
    }

    [Fact]
    public void RecordMeasurement_WhenCompleted_ShouldThrowBusinessRuleException()
    {
        var installation = CreateValidInstallation();
        installation.MarkAsCompleted();

        var act = () => installation.RecordMeasurement(
            MeasurementId.New(),
            MeasurementType.Voltage,
            new MeasurementValue(230.0, "V"));

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void RecordMeasurement_WithZeroValue_ShouldThrowBusinessRuleException()
    {
        var installation = CreateValidInstallation();

        var act = () => installation.RecordMeasurement(
            MeasurementId.New(),
            MeasurementType.Voltage,
            new MeasurementValue(0.0, "V"));

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void RecordMeasurement_WithNegativeValue_ShouldThrowBusinessRuleException()
    {
        var installation = CreateValidInstallation();

        var act = () => installation.RecordMeasurement(
            MeasurementId.New(),
            MeasurementType.Voltage,
            new MeasurementValue(-5.0, "V"));

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void RemoveMeasurement_ShouldRemoveFromCollection()
    {
        var installation = CreateValidInstallation();
        var measurementId = MeasurementId.New();
        installation.RecordMeasurement(measurementId, MeasurementType.Voltage, new MeasurementValue(230.0, "V"));
        installation.ClearDomainEvents();

        installation.RemoveMeasurement(measurementId);

        installation.Measurements.Should().BeEmpty();
    }

    [Fact]
    public void RemoveMeasurement_ShouldRaiseMeasurementRemovedEvent()
    {
        var installation = CreateValidInstallation();
        var measurementId = MeasurementId.New();
        installation.RecordMeasurement(measurementId, MeasurementType.Voltage, new MeasurementValue(230.0, "V"));
        installation.ClearDomainEvents();

        installation.RemoveMeasurement(measurementId);

        installation.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<MeasurementRemoved>()
            .Which.MeasurementId.Should().Be(measurementId);
    }

    [Fact]
    public void RemoveMeasurement_WithNonExistentId_ShouldThrowInvalidOperation()
    {
        var installation = CreateValidInstallation();

        var act = () => installation.RemoveMeasurement(MeasurementId.New());

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void RemoveMeasurement_WhenCompleted_ShouldThrowBusinessRuleException()
    {
        var installation = CreateValidInstallation();
        var measurementId = MeasurementId.New();
        installation.RecordMeasurement(measurementId, MeasurementType.Voltage, new MeasurementValue(230.0, "V"));
        installation.MarkAsCompleted();

        var act = () => installation.RemoveMeasurement(measurementId);

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void RecordMultipleMeasurements_ShouldTrackAll()
    {
        var installation = CreateValidInstallation();

        installation.RecordMeasurement(MeasurementId.New(), MeasurementType.Voltage, new MeasurementValue(230.0, "V"));
        installation.RecordMeasurement(MeasurementId.New(), MeasurementType.Continuity, new MeasurementValue(0.3, "Ohm"));
        installation.RecordMeasurement(MeasurementId.New(), MeasurementType.InsulationResistance, new MeasurementValue(500.0, "MOhm"));

        installation.Measurements.Should().HaveCount(3);
    }

    [Fact]
    public void RecordMeasurement_WithRcdTripCurrent_ShouldSucceed()
    {
        var installation = CreateValidInstallation();

        installation.RecordMeasurement(
            MeasurementId.New(),
            MeasurementType.RcdTripCurrent,
            new MeasurementValue(30.0, "mA", 15.0, 30.0));

        installation.Measurements.Should().ContainSingle();
        installation.Measurements[0].Type.Should().Be(MeasurementType.RcdTripCurrent);
    }
}
