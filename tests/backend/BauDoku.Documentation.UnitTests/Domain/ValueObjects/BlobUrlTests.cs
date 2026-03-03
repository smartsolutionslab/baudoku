using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class BlobUrlTests
{
    [Fact]
    public void From_WithValidUrl_ShouldSucceed()
    {
        var url = BlobUrl.From("https://storage.example.com/photos/abc.jpg");
        url.Value.Should().Be("https://storage.example.com/photos/abc.jpg");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => BlobUrl.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongValue_ShouldThrow()
    {
        var longUrl = new string('a', BlobUrl.MaxLength + 1);
        var act = () => BlobUrl.From(longUrl);
        act.Should().Throw<ArgumentException>();
    }
}
