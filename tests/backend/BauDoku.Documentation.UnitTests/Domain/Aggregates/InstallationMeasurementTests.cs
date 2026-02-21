using AwesomeAssertions;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain;
using BauDoku.Documentation.UnitTests.Builders;

namespace BauDoku.Documentation.UnitTests.Domain.Aggregates;

public sealed class InstallationMeasurementTests
{
    private static Installation CreateValidInstallation() => new InstallationBuilder().Build();

    [Fact]
    public void RecordMeasurement_ShouldAddMeasurementToCollection()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();
        var measurementId = MeasurementIdentifier.New();

        installation.RecordMeasurement(
            measurementId,
            MeasurementType.Voltage,
            MeasurementValue.Create(230.0, "V"),
            Notes.From("Spannungsmessung"));

        installation.Measurements.Should().ContainSingle();
        var measurement = installation.Measurements[0];
        measurement.Id.Should().Be(measurementId);
        measurement.Type.Should().Be(MeasurementType.Voltage);
        measurement.Value.Value.Should().Be(230.0);
        measurement.Value.Unit.Value.Should().Be("V");
        measurement.Notes!.Value.Should().Be("Spannungsmessung");
    }

    [Fact]
    public void RecordMeasurement_ShouldRaiseMeasurementRecordedEvent()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();
        var measurementId = MeasurementIdentifier.New();

        installation.RecordMeasurement(
            measurementId,
            MeasurementType.Voltage,
            MeasurementValue.Create(230.0, "V"));

        installation.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<MeasurementRecorded>()
            .Which.MeasurementIdentifier.Should().Be(measurementId);
    }

    [Fact]
    public void RecordMeasurement_WithThresholdsInRange_ShouldEvaluateAsPassed()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();

        installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.Voltage,
            MeasurementValue.Create(230.0, "V", 220.0, 240.0));

        var measurement = installation.Measurements[0];
        measurement.Result.Should().Be(MeasurementResult.Passed);
    }

    [Fact]
    public void RecordMeasurement_WithValueBelowMinThreshold_ShouldEvaluateAsFailed()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();

        installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.Voltage,
            MeasurementValue.Create(200.0, "V", 220.0, 240.0));

        var measurement = installation.Measurements[0];
        measurement.Result.Should().Be(MeasurementResult.Failed);
    }

    [Fact]
    public void RecordMeasurement_WithValueAboveMaxThreshold_ShouldEvaluateAsFailed()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();

        installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.Voltage,
            MeasurementValue.Create(250.0, "V", 220.0, 240.0));

        var measurement = installation.Measurements[0];
        measurement.Result.Should().Be(MeasurementResult.Failed);
    }

    [Fact]
    public void RecordMeasurement_WithoutThresholds_ShouldEvaluateAsPassed()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();

        installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.Voltage,
            MeasurementValue.Create(230.0, "V"));

        var measurement = installation.Measurements[0];
        measurement.Result.Should().Be(MeasurementResult.Passed);
    }

    [Fact]
    public void RecordMeasurement_WithValueAtMinThreshold_ShouldEvaluateAsPassed()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();

        installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.Voltage,
            MeasurementValue.Create(220.0, "V", 220.0, 240.0));

        var measurement = installation.Measurements[0];
        measurement.Result.Should().Be(MeasurementResult.Passed);
    }

    [Fact]
    public void RecordMeasurement_WithValueAtMaxThreshold_ShouldEvaluateAsPassed()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();

        installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.Voltage,
            MeasurementValue.Create(240.0, "V", 220.0, 240.0));

        var measurement = installation.Measurements[0];
        measurement.Result.Should().Be(MeasurementResult.Passed);
    }

    [Fact]
    public void RecordMeasurement_WithOnlyMinThresholdAndValueAbove_ShouldPass()
    {
        var installation = CreateValidInstallation();

        installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.InsulationResistance,
            MeasurementValue.Create(1.5, "MOhm", minThreshold: 1.0));

        installation.Measurements[0].Result.Should().Be(MeasurementResult.Passed);
    }

    [Fact]
    public void RecordMeasurement_WithOnlyMinThresholdAndValueBelow_ShouldFail()
    {
        var installation = CreateValidInstallation();

        installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.InsulationResistance,
            MeasurementValue.Create(0.5, "MOhm", minThreshold: 1.0));

        installation.Measurements[0].Result.Should().Be(MeasurementResult.Failed);
    }

    [Fact]
    public void RecordMeasurement_WithOptionalNotesNull_ShouldSucceed()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();

        installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.Continuity,
            MeasurementValue.Create(0.5, "Ohm"));

        installation.Measurements[0].Notes.Should().BeNull();
    }

    [Fact]
    public void RecordMeasurement_WhenCompleted_ShouldThrowBusinessRuleException()
    {
        var installation = CreateValidInstallation();
        installation.MarkAsCompleted();

        var act = () => installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.Voltage,
            MeasurementValue.Create(230.0, "V"));

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void RecordMeasurement_WithZeroValue_ShouldSucceed()
    {
        var installation = CreateValidInstallation();

        installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.Voltage,
            MeasurementValue.Create(0.0, "V"));

        installation.Measurements.Should().ContainSingle();
    }

    [Fact]
    public void RecordMeasurement_WithNegativeValue_ShouldThrowBusinessRuleException()
    {
        var installation = CreateValidInstallation();

        var act = () => installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.Voltage,
            MeasurementValue.Create(-5.0, "V"));

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void RemoveMeasurement_ShouldRemoveFromCollection()
    {
        var installation = CreateValidInstallation();
        var measurementId = MeasurementIdentifier.New();
        installation.RecordMeasurement(measurementId, MeasurementType.Voltage, MeasurementValue.Create(230.0, "V"));
        installation.ClearDomainEvents();

        installation.RemoveMeasurement(measurementId);

        installation.Measurements.Should().BeEmpty();
    }

    [Fact]
    public void RemoveMeasurement_ShouldRaiseMeasurementRemovedEvent()
    {
        var installation = CreateValidInstallation();
        var measurementId = MeasurementIdentifier.New();
        installation.RecordMeasurement(measurementId, MeasurementType.Voltage, MeasurementValue.Create(230.0, "V"));
        installation.ClearDomainEvents();

        installation.RemoveMeasurement(measurementId);

        installation.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<MeasurementRemoved>()
            .Which.MeasurementIdentifier.Should().Be(measurementId);
    }

    [Fact]
    public void RemoveMeasurement_WithNonExistentId_ShouldThrowInvalidOperation()
    {
        var installation = CreateValidInstallation();

        var act = () => installation.RemoveMeasurement(MeasurementIdentifier.New());

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void RemoveMeasurement_WhenCompleted_ShouldThrowBusinessRuleException()
    {
        var installation = CreateValidInstallation();
        var measurementId = MeasurementIdentifier.New();
        installation.RecordMeasurement(measurementId, MeasurementType.Voltage, MeasurementValue.Create(230.0, "V"));
        installation.MarkAsCompleted();

        var act = () => installation.RemoveMeasurement(measurementId);

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void RecordMultipleMeasurements_ShouldTrackAll()
    {
        var installation = CreateValidInstallation();

        installation.RecordMeasurement(MeasurementIdentifier.New(), MeasurementType.Voltage, MeasurementValue.Create(230.0, "V"));
        installation.RecordMeasurement(MeasurementIdentifier.New(), MeasurementType.Continuity, MeasurementValue.Create(0.3, "Ohm"));
        installation.RecordMeasurement(MeasurementIdentifier.New(), MeasurementType.InsulationResistance, MeasurementValue.Create(500.0, "MOhm"));

        installation.Measurements.Should().HaveCount(3);
    }

    [Fact]
    public void RecordMeasurement_WithRcdTripCurrent_OnSwitchgear_ShouldSucceed()
    {
        var installation = CreateElectricalInstallation();

        installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.RcdTripCurrent,
            MeasurementValue.Create(30.0, "mA", 15.0, 30.0));

        installation.Measurements.Should().ContainSingle();
        installation.Measurements[0].Type.Should().Be(MeasurementType.RcdTripCurrent);
    }

    [Fact]
    public void RecordMeasurement_WithRcdTripTime_OnCableTray_ShouldThrowBusinessRuleException()
    {
        var installation = CreateValidInstallation();

        var act = () => installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.RcdTripTime,
            MeasurementValue.Create(25.0, "ms"));

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void RecordMeasurement_WithRcdTripCurrent_OnCableTray_ShouldThrowBusinessRuleException()
    {
        var installation = CreateValidInstallation();

        var act = () => installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.RcdTripCurrent,
            MeasurementValue.Create(30.0, "mA"));

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void RecordMeasurement_WithLoopImpedance_OnCableTray_ShouldThrowBusinessRuleException()
    {
        var installation = CreateValidInstallation();

        var act = () => installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.LoopImpedance,
            MeasurementValue.Create(0.5, "Ohm"));

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void RecordMeasurement_WithVoltage_OnCableTray_ShouldSucceed()
    {
        var installation = CreateValidInstallation();

        installation.RecordMeasurement(
            MeasurementIdentifier.New(),
            MeasurementType.Voltage,
            MeasurementValue.Create(230.0, "V"));

        installation.Measurements.Should().ContainSingle();
    }

    private static Installation CreateElectricalInstallation() =>
        new InstallationBuilder()
            .WithType(InstallationType.Switchgear)
            .WithDescription(Description.From("Schaltanlage im Keller"))
            .Build();
}
