using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class MeasurementValueExtendedTests
{
    [Fact]
    public void Create_WithThresholds_ShouldStoreValues()
    {
        var value = new MeasurementValue(5.0, "mA", 1.0, 10.0);

        value.Value.Should().Be(5.0);
        value.Unit.Should().Be("mA");
        value.MinThreshold.Should().Be(1.0);
        value.MaxThreshold.Should().Be(10.0);
    }

    [Fact]
    public void Create_WithoutThresholds_ShouldDefaultToNull()
    {
        var value = new MeasurementValue(5.0, "mA");

        value.MinThreshold.Should().BeNull();
        value.MaxThreshold.Should().BeNull();
    }

    [Fact]
    public void Create_WithOnlyMinThreshold_ShouldWork()
    {
        var value = new MeasurementValue(5.0, "V", minThreshold: 1.0);

        value.MinThreshold.Should().Be(1.0);
        value.MaxThreshold.Should().BeNull();
    }

    [Fact]
    public void Create_WithOnlyMaxThreshold_ShouldWork()
    {
        var value = new MeasurementValue(5.0, "V", maxThreshold: 10.0);

        value.MinThreshold.Should().BeNull();
        value.MaxThreshold.Should().Be(10.0);
    }

    [Fact]
    public void Create_WithEmptyUnit_ShouldThrow()
    {
        var act = () => new MeasurementValue(5.0, "");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithUnitExceedingMaxLength_ShouldThrow()
    {
        var act = () => new MeasurementValue(5.0, new string('x', MeasurementValue.MaxUnitLength + 1));
        act.Should().Throw<ArgumentException>();
    }
}
