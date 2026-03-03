using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class MeasurementValueExtendedTests
{
    [Fact]
    public void Create_WithThresholds_ShouldStoreValues()
    {
        var value = MeasurementValue.Create(5.0, MeasurementUnit.From("mA"), 1.0, 10.0);

        value.Value.Should().Be(5.0);
        value.Unit.Value.Should().Be("mA");
        value.MinThreshold.Should().Be(1.0);
        value.MaxThreshold.Should().Be(10.0);
    }

    [Fact]
    public void Create_WithoutThresholds_ShouldDefaultToNull()
    {
        var value = MeasurementValue.Create(5.0, MeasurementUnit.From("mA"));

        value.MinThreshold.Should().BeNull();
        value.MaxThreshold.Should().BeNull();
    }

    [Fact]
    public void Create_WithOnlyMinThreshold_ShouldWork()
    {
        var value = MeasurementValue.Create(5.0, MeasurementUnit.From("V"), minThreshold: 1.0);

        value.MinThreshold.Should().Be(1.0);
        value.MaxThreshold.Should().BeNull();
    }

    [Fact]
    public void Create_WithOnlyMaxThreshold_ShouldWork()
    {
        var value = MeasurementValue.Create(5.0, MeasurementUnit.From("V"), maxThreshold: 10.0);

        value.MinThreshold.Should().BeNull();
        value.MaxThreshold.Should().Be(10.0);
    }

    [Fact]
    public void Create_WithEmptyUnit_ShouldThrow()
    {
        var act = () => MeasurementValue.Create(5.0, MeasurementUnit.From(""));
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithUnitExceedingMaxLength_ShouldThrow()
    {
        var act = () => MeasurementValue.Create(5.0, MeasurementUnit.From(new string('x', MeasurementUnit.MaxLength + 1)));
        act.Should().Throw<ArgumentException>();
    }
}
