using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class DepthTests
{
    [Fact]
    public void Create_WithValidDepth_ShouldSucceed()
    {
        var depth = Depth.From(600);
        depth.Value.Should().Be(600);
    }

    [Fact]
    public void Create_WithZeroDepth_ShouldSucceed()
    {
        var depth = Depth.From(0);
        depth.Value.Should().Be(0);
    }

    [Fact]
    public void Create_WithNegativeDepth_ShouldThrow()
    {
        var act = () => Depth.From(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
