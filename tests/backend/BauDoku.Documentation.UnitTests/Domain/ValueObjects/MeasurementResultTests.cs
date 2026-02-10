using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class MeasurementResultTests
{
    [Theory]
    [InlineData("passed")]
    [InlineData("failed")]
    [InlineData("warning")]
    public void Create_WithValidValue_ShouldSucceed(string value)
    {
        var result = new MeasurementResult(value);
        result.Value.Should().Be(value);
    }

    [Fact]
    public void Create_WithInvalidValue_ShouldThrow()
    {
        var act = () => new MeasurementResult("invalid");
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => new MeasurementResult(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void StaticInstances_ShouldHaveCorrectValues()
    {
        MeasurementResult.Passed.Value.Should().Be("passed");
        MeasurementResult.Failed.Value.Should().Be("failed");
        MeasurementResult.Warning.Value.Should().Be("warning");
    }
}
