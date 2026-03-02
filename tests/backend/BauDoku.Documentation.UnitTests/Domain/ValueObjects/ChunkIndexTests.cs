using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class ChunkIndexTests
{
    [Fact]
    public void From_WithPositiveValue_ShouldSucceed()
    {
        var index = ChunkIndex.From(3);
        index.Value.Should().Be(3);
    }

    [Fact]
    public void From_WithZero_ShouldSucceed()
    {
        var index = ChunkIndex.From(0);
        index.Value.Should().Be(0);
    }

    [Fact]
    public void From_WithNegativeValue_ShouldThrow()
    {
        var act = () => ChunkIndex.From(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
