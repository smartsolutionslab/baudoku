using AwesomeAssertions;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.BuildingBlocks.UnitTests.Guards;

public sealed class EnsureNullableNumericGuardTests
{
    [Fact]
    public void IsPositive_WithNull_ShouldNotThrow()
    {
        Action act = () => Ensure.That((int?)null).IsPositive();
        act.Should().NotThrow();
    }

    [Fact]
    public void IsPositive_WithPositive_ShouldNotThrow()
    {
        Action act = () => Ensure.That((int?)5).IsPositive();
        act.Should().NotThrow();
    }

    [Fact]
    public void IsPositive_WithZero_ShouldThrow()
    {
        Action act = () => Ensure.That((int?)0).IsPositive();
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void IsNotNegative_WithNull_ShouldNotThrow()
    {
        Action act = () => Ensure.That((int?)null).IsNotNegative();
        act.Should().NotThrow();
    }

    [Fact]
    public void IsNotNegative_WithNegative_ShouldThrow()
    {
        Action act = () => Ensure.That((int?)-1).IsNotNegative();
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void IsBetween_WithNull_ShouldNotThrow()
    {
        Action act = () => Ensure.That((double?)null).IsBetween(-90.0, 90.0);
        act.Should().NotThrow();
    }

    [Fact]
    public void IsBetween_WithValueInRange_ShouldNotThrow()
    {
        Action act = () => Ensure.That((double?)45.0).IsBetween(-90.0, 90.0);
        act.Should().NotThrow();
    }

    [Fact]
    public void IsBetween_WithValueOutOfRange_ShouldThrow()
    {
        Action act = () => Ensure.That((double?)91.0).IsBetween(-90.0, 90.0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void IsGreaterThan_WithNull_ShouldNotThrow()
    {
        Action act = () => Ensure.That((int?)null).IsGreaterThan(0);
        act.Should().NotThrow();
    }

    [Fact]
    public void IsGreaterThan_WithValueGreater_ShouldNotThrow()
    {
        Action act = () => Ensure.That((int?)5).IsGreaterThan(0);
        act.Should().NotThrow();
    }

    [Fact]
    public void IsGreaterThan_WithValueNotGreater_ShouldThrow()
    {
        Action act = () => Ensure.That((int?)0).IsGreaterThan(0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void NullableLong_IsNotNegative_WithNull_ShouldNotThrow()
    {
        Action act = () => Ensure.That((long?)null).IsNotNegative();
        act.Should().NotThrow();
    }

    [Fact]
    public void NullableDouble_IsPositive_WithNegative_ShouldThrow()
    {
        Action act = () => Ensure.That((double?)-1.5).IsPositive();
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
