using AwesomeAssertions;
using BauDoku.Documentation.Domain.Rules;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.Rules;

public sealed class MeasurementTypeMustMatchInstallationTypeTests
{
    [Theory]
    [InlineData("switchgear")]
    [InlineData("transformer")]
    [InlineData("junction_box")]
    public void RcdTripTime_OnElectricalInstallation_ShouldNotBeBroken(string installationType)
    {
        var rule = new MeasurementTypeMustMatchInstallationType(
            new InstallationType(installationType),
            MeasurementType.RcdTripTime);

        rule.IsBroken().Should().BeFalse();
    }

    [Theory]
    [InlineData("cable_tray")]
    [InlineData("cable_pull")]
    [InlineData("conduit")]
    [InlineData("grounding")]
    [InlineData("lightning_protection")]
    [InlineData("other")]
    public void RcdTripTime_OnNonElectricalInstallation_ShouldBeBroken(string installationType)
    {
        var rule = new MeasurementTypeMustMatchInstallationType(
            new InstallationType(installationType),
            MeasurementType.RcdTripTime);

        rule.IsBroken().Should().BeTrue();
    }

    [Theory]
    [InlineData("switchgear")]
    [InlineData("transformer")]
    [InlineData("junction_box")]
    public void RcdTripCurrent_OnElectricalInstallation_ShouldNotBeBroken(string installationType)
    {
        var rule = new MeasurementTypeMustMatchInstallationType(
            new InstallationType(installationType),
            MeasurementType.RcdTripCurrent);

        rule.IsBroken().Should().BeFalse();
    }

    [Theory]
    [InlineData("cable_tray")]
    [InlineData("conduit")]
    public void RcdTripCurrent_OnNonElectricalInstallation_ShouldBeBroken(string installationType)
    {
        var rule = new MeasurementTypeMustMatchInstallationType(
            new InstallationType(installationType),
            MeasurementType.RcdTripCurrent);

        rule.IsBroken().Should().BeTrue();
    }

    [Theory]
    [InlineData("switchgear")]
    [InlineData("transformer")]
    [InlineData("junction_box")]
    [InlineData("grounding")]
    [InlineData("lightning_protection")]
    public void LoopImpedance_OnAllowedInstallation_ShouldNotBeBroken(string installationType)
    {
        var rule = new MeasurementTypeMustMatchInstallationType(
            new InstallationType(installationType),
            MeasurementType.LoopImpedance);

        rule.IsBroken().Should().BeFalse();
    }

    [Theory]
    [InlineData("cable_tray")]
    [InlineData("cable_pull")]
    [InlineData("conduit")]
    [InlineData("other")]
    public void LoopImpedance_OnNonAllowedInstallation_ShouldBeBroken(string installationType)
    {
        var rule = new MeasurementTypeMustMatchInstallationType(
            new InstallationType(installationType),
            MeasurementType.LoopImpedance);

        rule.IsBroken().Should().BeTrue();
    }

    [Theory]
    [InlineData("cable_tray")]
    [InlineData("junction_box")]
    [InlineData("switchgear")]
    [InlineData("conduit")]
    [InlineData("grounding")]
    [InlineData("other")]
    public void Voltage_OnAnyInstallation_ShouldNotBeBroken(string installationType)
    {
        var rule = new MeasurementTypeMustMatchInstallationType(
            new InstallationType(installationType),
            MeasurementType.Voltage);

        rule.IsBroken().Should().BeFalse();
    }

    [Theory]
    [InlineData("cable_tray")]
    [InlineData("junction_box")]
    [InlineData("switchgear")]
    [InlineData("other")]
    public void InsulationResistance_OnAnyInstallation_ShouldNotBeBroken(string installationType)
    {
        var rule = new MeasurementTypeMustMatchInstallationType(
            new InstallationType(installationType),
            MeasurementType.InsulationResistance);

        rule.IsBroken().Should().BeFalse();
    }

    [Theory]
    [InlineData("cable_tray")]
    [InlineData("conduit")]
    [InlineData("other")]
    public void Continuity_OnAnyInstallation_ShouldNotBeBroken(string installationType)
    {
        var rule = new MeasurementTypeMustMatchInstallationType(
            new InstallationType(installationType),
            MeasurementType.Continuity);

        rule.IsBroken().Should().BeFalse();
    }

    [Fact]
    public void Message_ShouldContainBothTypes()
    {
        var rule = new MeasurementTypeMustMatchInstallationType(
            InstallationType.CableTray,
            MeasurementType.RcdTripTime);

        rule.Message.Should().Contain("rcd_trip_time");
        rule.Message.Should().Contain("cable_tray");
    }
}
