using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class ConductorCountTests
{
    [Fact]
    public void From_WithPositiveValue_ShouldSucceed()
    {
        var count = ConductorCount.From(5);
        count.Value.Should().Be(5);
    }

    [Fact]
    public void From_WithZero_ShouldThrow()
    {
        var act = () => ConductorCount.From(0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void From_WithNegativeValue_ShouldThrow()
    {
        var act = () => ConductorCount.From(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnConductorCount()
    {
        var count = ConductorCount.FromNullable(3);
        count.Should().NotBeNull();
        count!.Value.Should().Be(3);
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        var count = ConductorCount.FromNullable(null);
        count.Should().BeNull();
    }
}
