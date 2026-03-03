using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class CableColorTests
{
    [Fact]
    public void From_WithValidColor_ShouldSucceed()
    {
        var color = CableColor.From("schwarz");
        color.Value.Should().Be("schwarz");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => CableColor.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongValue_ShouldThrow()
    {
        var longValue = new string('a', CableColor.MaxLength + 1);
        var act = () => CableColor.From(longValue);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        var result = CableColor.FromNullable(null);
        result.Should().BeNull();
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnInstance()
    {
        var result = CableColor.FromNullable("rot");
        result.Should().NotBeNull();
        result!.Value.Should().Be("rot");
    }
}
