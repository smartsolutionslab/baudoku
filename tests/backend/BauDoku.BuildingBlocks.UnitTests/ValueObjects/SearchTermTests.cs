using AwesomeAssertions;
using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.BuildingBlocks.UnitTests.ValueObjects;

public sealed class SearchTermTests
{
    [Fact]
    public void From_WithValidString_ShouldCreateSearchTerm()
    {
        var term = SearchTerm.From("Kabel");

        term.Value.Should().Be("Kabel");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyOrWhitespace_ShouldThrow(string? value)
    {
        Action act = () => SearchTerm.From(value!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_ExceedingMaxLength_ShouldThrow()
    {
        var tooLong = new string('a', SearchTerm.MaxLength + 1);

        Action act = () => SearchTerm.From(tooLong);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_AtMaxLength_ShouldSucceed()
    {
        var atLimit = new string('a', SearchTerm.MaxLength);

        var term = SearchTerm.From(atLimit);

        term.Value.Should().HaveLength(SearchTerm.MaxLength);
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        var result = SearchTerm.FromNullable(null);

        result.Should().BeNull();
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnSearchTerm()
    {
        var result = SearchTerm.FromNullable("Kabel");

        result.Should().NotBeNull();
        result!.Value.Should().Be("Kabel");
    }
}
