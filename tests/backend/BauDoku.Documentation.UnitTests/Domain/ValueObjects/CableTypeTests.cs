using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class CableTypeTests
{
    [Fact]
    public void From_WithValidType_ShouldSucceed()
    {
        var type = CableType.From("NYM-J 5x2.5");
        type.Value.Should().Be("NYM-J 5x2.5");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => CableType.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongValue_ShouldThrow()
    {
        var longValue = new string('a', CableType.MaxLength + 1);
        var act = () => CableType.From(longValue);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        CableType.FromNullable(null).Should().BeNull();
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnInstance()
    {
        CableType.FromNullable("NYY-J 3x1.5")!.Value.Should().Be("NYY-J 3x1.5");
    }
}
