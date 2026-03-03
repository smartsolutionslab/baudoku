using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class MeasurementValueTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var unit = MeasurementUnit.From("V");
        var mv = MeasurementValue.Create(230.0, unit);

        mv.Value.Should().Be(230.0);
        mv.Unit.Should().Be(unit);
        mv.MinThreshold.Should().BeNull();
        mv.MaxThreshold.Should().BeNull();
    }

    [Fact]
    public void Create_WithThresholds_ShouldSetThresholds()
    {
        var unit = MeasurementUnit.From("MΩ");
        var mv = MeasurementValue.Create(1.5, unit, minThreshold: 0.5, maxThreshold: 10.0);

        mv.Value.Should().Be(1.5);
        mv.MinThreshold.Should().Be(0.5);
        mv.MaxThreshold.Should().Be(10.0);
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldSucceed()
    {
        var unit = MeasurementUnit.From("°C");
        var mv = MeasurementValue.Create(-5.0, unit);

        mv.Value.Should().Be(-5.0);
    }
}
