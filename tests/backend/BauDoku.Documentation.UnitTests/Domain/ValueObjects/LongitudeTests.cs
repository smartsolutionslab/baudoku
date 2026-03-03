using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class LongitudeTests
{
    [Theory]
    [InlineData(0.0)]
    [InlineData(11.576124)]
    [InlineData(-180.0)]
    [InlineData(180.0)]
    public void From_WithValidValue_ShouldSucceed(double value)
    {
        var lon = Longitude.From(value);
        lon.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(-180.1)]
    [InlineData(180.1)]
    [InlineData(-360.0)]
    [InlineData(360.0)]
    public void From_WithOutOfRange_ShouldThrow(double value)
    {
        var act = () => Longitude.From(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        Longitude.FromNullable(null).Should().BeNull();
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnInstance()
    {
        Longitude.FromNullable(13.405)!.Value.Should().Be(13.405);
    }
}
