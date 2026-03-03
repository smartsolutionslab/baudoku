using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class ContentTypeTests
{
    [Fact]
    public void From_WithValidContentType_ShouldSucceed()
    {
        var ct = ContentType.From("image/jpeg");
        ct.Value.Should().Be("image/jpeg");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => ContentType.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongValue_ShouldThrow()
    {
        var longValue = new string('a', ContentType.MaxLength + 1);
        var act = () => ContentType.From(longValue);
        act.Should().Throw<ArgumentException>();
    }
}
