using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class RtkFixStatusTests
{
    [Fact]
    public void From_WithValidStatus_ShouldSucceed()
    {
        var status = RtkFixStatus.From("fixed");
        status.Value.Should().Be("fixed");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => RtkFixStatus.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongValue_ShouldThrow()
    {
        var longValue = new string('a', RtkFixStatus.MaxLength + 1);
        var act = () => RtkFixStatus.From(longValue);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        RtkFixStatus.FromNullable(null).Should().BeNull();
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnInstance()
    {
        RtkFixStatus.FromNullable("float")!.Value.Should().Be("float");
    }
}
