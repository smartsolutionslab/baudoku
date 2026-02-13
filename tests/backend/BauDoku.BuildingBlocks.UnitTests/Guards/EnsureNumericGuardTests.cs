using AwesomeAssertions;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.BuildingBlocks.UnitTests.Guards;

public sealed class EnsureNumericGuardTests
{
    [Fact]
    public void IsGreaterThan_WhenGreater_ShouldNotThrow()
    {
        Action act = () => Ensure.That(10).IsGreaterThan(5);
        act.Should().NotThrow();
    }

    [Fact]
    public void IsGreaterThan_WhenEqual_ShouldThrow()
    {
        Action act = () => Ensure.That(5).IsGreaterThan(5);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void IsGreaterThan_WhenLess_ShouldThrow()
    {
        Action act = () => Ensure.That(3).IsGreaterThan(5);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void IsNotNegative_WithPositive_ShouldNotThrow()
    {
        Action act = () => Ensure.That(5).IsNotNegative();
        act.Should().NotThrow();
    }

    [Fact]
    public void IsNotNegative_WithZero_ShouldNotThrow()
    {
        Action act = () => Ensure.That(0).IsNotNegative();
        act.Should().NotThrow();
    }

    [Fact]
    public void IsNotNegative_WithNegative_ShouldThrow()
    {
        Action act = () => Ensure.That(-1).IsNotNegative();
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void IsPositive_WithPositive_ShouldNotThrow()
    {
        Action act = () => Ensure.That(1).IsPositive();
        act.Should().NotThrow();
    }

    [Fact]
    public void IsPositive_WithZero_ShouldThrow()
    {
        Action act = () => Ensure.That(0).IsPositive();
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void IsBetween_WithinRange_ShouldNotThrow()
    {
        Action act = () => Ensure.That(5.0).IsBetween(-90.0, 90.0);
        act.Should().NotThrow();
    }

    [Fact]
    public void IsBetween_AtLowerBound_ShouldNotThrow()
    {
        Action act = () => Ensure.That(-90.0).IsBetween(-90.0, 90.0);
        act.Should().NotThrow();
    }

    [Fact]
    public void IsBetween_AtUpperBound_ShouldNotThrow()
    {
        Action act = () => Ensure.That(90.0).IsBetween(-90.0, 90.0);
        act.Should().NotThrow();
    }

    [Fact]
    public void IsBetween_BelowRange_ShouldThrow()
    {
        Action act = () => Ensure.That(-91.0).IsBetween(-90.0, 90.0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void IsBetween_AboveRange_ShouldThrow()
    {
        Action act = () => Ensure.That(91.0).IsBetween(-90.0, 90.0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Chaining_MultipleNumericGuards_ShouldWork()
    {
        Action act = () => Ensure.That(5).IsNotNegative().IsGreaterThan(0).IsBetween(1, 10);
        act.Should().NotThrow();
    }

    [Fact]
    public void Long_IsNotNegative_ShouldWork()
    {
        Action act = () => Ensure.That(100L).IsNotNegative();
        act.Should().NotThrow();
    }

    [Fact]
    public void Long_Negative_ShouldThrow()
    {
        Action act = () => Ensure.That(-1L).IsNotNegative();
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
