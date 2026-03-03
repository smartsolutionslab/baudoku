using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class LatitudeTests
{
    [Theory]
    [InlineData(0.0)]
    [InlineData(48.137154)]
    [InlineData(-90.0)]
    [InlineData(90.0)]
    public void From_WithValidValue_ShouldSucceed(double value)
    {
        var lat = Latitude.From(value);
        lat.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(-90.1)]
    [InlineData(90.1)]
    [InlineData(-180.0)]
    [InlineData(180.0)]
    public void From_WithOutOfRange_ShouldThrow(double value)
    {
        var act = () => Latitude.From(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        Latitude.FromNullable(null).Should().BeNull();
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnInstance()
    {
        Latitude.FromNullable(52.52)!.Value.Should().Be(52.52);
    }
}
