using AwesomeAssertions;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.BuildingBlocks.UnitTests.Guards;

public sealed class EnsureStringGuardTests
{
    [Fact]
    public void IsNotNullOrWhiteSpace_WithValidString_ShouldNotThrow()
    {
        Action act = () => Ensure.That("hello").IsNotNullOrWhiteSpace();
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void IsNotNullOrWhiteSpace_WithInvalidString_ShouldThrow(string? value)
    {
        Action act = () => Ensure.That(value).IsNotNullOrWhiteSpace();
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MaxLengthIs_WithinLimit_ShouldNotThrow()
    {
        Action act = () => Ensure.That("hello").MaxLengthIs(10);
        act.Should().NotThrow();
    }

    [Fact]
    public void MaxLengthIs_ExceedingLimit_ShouldThrow()
    {
        Action act = () => Ensure.That("hello world").MaxLengthIs(5);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MaxLengthIs_NullValue_ShouldNotThrow()
    {
        Action act = () => Ensure.That((string?)null).MaxLengthIs(5);
        act.Should().NotThrow();
    }

    [Fact]
    public void MinLengthIs_MeetsMinimum_ShouldNotThrow()
    {
        Action act = () => Ensure.That("hello").MinLengthIs(3);
        act.Should().NotThrow();
    }

    [Fact]
    public void MinLengthIs_BelowMinimum_ShouldThrow()
    {
        Action act = () => Ensure.That("hi").MinLengthIs(3);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void IsOneOf_ValidValue_ShouldNotThrow()
    {
        var validValues = new HashSet<string> { "a", "b", "c" };
        Action act = () => Ensure.That("b").IsOneOf(validValues);
        act.Should().NotThrow();
    }

    [Fact]
    public void IsOneOf_InvalidValue_ShouldThrow()
    {
        var validValues = new HashSet<string> { "a", "b", "c" };
        Action act = () => Ensure.That("d").IsOneOf(validValues);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Chaining_MultipleGuards_ShouldWork()
    {
        Action act = () => Ensure.That("hello").IsNotNullOrWhiteSpace().MaxLengthIs(200).MinLengthIs(1);
        act.Should().NotThrow();
    }

    [Fact]
    public void Chaining_FirstGuardFails_ShouldThrow()
    {
        Action act = () => Ensure.That("").IsNotNullOrWhiteSpace().MaxLengthIs(200);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MatchesPattern_ValidPattern_ShouldNotThrow()
    {
        var pattern = new System.Text.RegularExpressions.Regex(@"^\d+$");
        Action act = () => Ensure.That("123").MatchesPattern(pattern);
        act.Should().NotThrow();
    }

    [Fact]
    public void MatchesPattern_InvalidPattern_ShouldThrow()
    {
        var pattern = new System.Text.RegularExpressions.Regex(@"^\d+$");
        Action act = () => Ensure.That("abc").MatchesPattern(pattern);
        act.Should().Throw<ArgumentException>();
    }
}
