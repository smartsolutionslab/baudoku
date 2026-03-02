using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class RadiusMetersTests
{
    [Fact]
    public void From_WithPositiveValue_ShouldSucceed()
    {
        var radius = RadiusMeters.From(500.0);
        radius.Value.Should().Be(500.0);
    }

    [Fact]
    public void From_WithZero_ShouldThrow()
    {
        var act = () => RadiusMeters.From(0.0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void From_WithNegativeValue_ShouldThrow()
    {
        var act = () => RadiusMeters.From(-100.0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
