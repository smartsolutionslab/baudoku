using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class DepthTests
{
    [Fact]
    public void Create_WithValidDepth_ShouldSucceed()
    {
        var depth = new Depth(600);
        depth.ValueInMillimeters.Should().Be(600);
    }

    [Fact]
    public void Create_WithZeroDepth_ShouldSucceed()
    {
        var depth = new Depth(0);
        depth.ValueInMillimeters.Should().Be(0);
    }

    [Fact]
    public void Create_WithNegativeDepth_ShouldThrow()
    {
        var act = () => new Depth(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
