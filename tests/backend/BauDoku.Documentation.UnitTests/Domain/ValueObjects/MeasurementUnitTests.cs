using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class MeasurementUnitTests
{
    [Fact]
    public void From_WithValidUnit_ShouldSucceed()
    {
        var unit = MeasurementUnit.From("MΩ");
        unit.Value.Should().Be("MΩ");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => MeasurementUnit.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongValue_ShouldThrow()
    {
        var longValue = new string('a', MeasurementUnit.MaxLength + 1);
        var act = () => MeasurementUnit.From(longValue);
        act.Should().Throw<ArgumentException>();
    }
}
