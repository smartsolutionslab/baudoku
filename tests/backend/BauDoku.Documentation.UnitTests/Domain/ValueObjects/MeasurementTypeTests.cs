using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class MeasurementTypeTests
{
    [Theory]
    [InlineData("insulation_resistance")]
    [InlineData("continuity")]
    [InlineData("loop_impedance")]
    [InlineData("rcd_trip_time")]
    [InlineData("rcd_trip_current")]
    [InlineData("voltage")]
    [InlineData("other")]
    public void Create_WithValidType_ShouldSucceed(string value)
    {
        var type = new MeasurementType(value);

        type.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyType_ShouldThrow(string? value)
    {
        var act = () => new MeasurementType(value!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithInvalidType_ShouldThrow()
    {
        var act = () => new MeasurementType("invalid");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void StaticInstances_ShouldHaveCorrectValues()
    {
        MeasurementType.InsulationResistance.Value.Should().Be("insulation_resistance");
        MeasurementType.Continuity.Value.Should().Be("continuity");
        MeasurementType.LoopImpedance.Value.Should().Be("loop_impedance");
        MeasurementType.RcdTripTime.Value.Should().Be("rcd_trip_time");
        MeasurementType.RcdTripCurrent.Value.Should().Be("rcd_trip_current");
        MeasurementType.Voltage.Value.Should().Be("voltage");
        MeasurementType.Other.Value.Should().Be("other");
    }
}
