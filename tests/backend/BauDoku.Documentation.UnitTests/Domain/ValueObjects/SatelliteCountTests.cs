using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class SatelliteCountTests
{
    [Fact]
    public void From_WithPositiveValue_ShouldSucceed()
    {
        var count = SatelliteCount.From(14);
        count.Value.Should().Be(14);
    }

    [Fact]
    public void From_WithZero_ShouldSucceed()
    {
        var count = SatelliteCount.From(0);
        count.Value.Should().Be(0);
    }

    [Fact]
    public void From_WithNegativeValue_ShouldThrow()
    {
        var act = () => SatelliteCount.From(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnSatelliteCount()
    {
        var count = SatelliteCount.FromNullable(10);
        count.Should().NotBeNull();
        count!.Value.Should().Be(10);
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        var count = SatelliteCount.FromNullable(null);
        count.Should().BeNull();
    }
}
