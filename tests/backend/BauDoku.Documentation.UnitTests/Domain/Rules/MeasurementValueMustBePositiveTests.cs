using AwesomeAssertions;
using BauDoku.Documentation.Domain.Rules;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.Rules;

public sealed class MeasurementValueMustBePositiveTests
{
    [Fact]
    public void IsBroken_WithPositiveValue_ShouldReturnFalse()
    {
        var value = MeasurementValue.Create(5.0, "V");
        var rule = new MeasurementValueMustBePositive(value);

        rule.IsBroken().Should().BeFalse();
    }

    [Fact]
    public void IsBroken_WithZeroValue_ShouldReturnTrue()
    {
        var value = MeasurementValue.Create(0.0, "V");
        var rule = new MeasurementValueMustBePositive(value);

        rule.IsBroken().Should().BeTrue();
    }

    [Fact]
    public void IsBroken_WithNegativeValue_ShouldReturnTrue()
    {
        var value = MeasurementValue.Create(-1.0, "V");
        var rule = new MeasurementValueMustBePositive(value);

        rule.IsBroken().Should().BeTrue();
    }
}
