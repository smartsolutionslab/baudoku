using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class HorizontalAccuracyTests
{
    [Fact]
    public void From_WithValidValue_ShouldSucceed()
    {
        var accuracy = HorizontalAccuracy.From(3.5);
        accuracy.Value.Should().Be(3.5);
    }

    [Fact]
    public void From_WithZero_ShouldThrow()
    {
        var act = () => HorizontalAccuracy.From(0.0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void From_WithNegative_ShouldThrow()
    {
        var act = () => HorizontalAccuracy.From(-1.0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        HorizontalAccuracy.FromNullable(null).Should().BeNull();
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnInstance()
    {
        HorizontalAccuracy.FromNullable(0.5)!.Value.Should().Be(0.5);
    }
}
