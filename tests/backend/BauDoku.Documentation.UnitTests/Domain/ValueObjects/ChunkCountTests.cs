using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class ChunkCountTests
{
    [Fact]
    public void From_WithPositiveValue_ShouldSucceed()
    {
        var count = ChunkCount.From(5);
        count.Value.Should().Be(5);
    }

    [Fact]
    public void From_WithZero_ShouldThrow()
    {
        var act = () => ChunkCount.From(0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void From_WithNegativeValue_ShouldThrow()
    {
        var act = () => ChunkCount.From(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
