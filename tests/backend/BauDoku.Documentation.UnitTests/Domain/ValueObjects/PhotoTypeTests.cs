using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class PhotoTypeTests
{
    [Theory]
    [InlineData("before")]
    [InlineData("after")]
    [InlineData("detail")]
    [InlineData("overview")]
    [InlineData("document")]
    [InlineData("other")]
    public void Create_WithValidType_ShouldSucceed(string value)
    {
        var photoType = PhotoType.From(value);
        photoType.Value.Should().Be(value);
    }

    [Fact]
    public void Create_WithInvalidType_ShouldThrow()
    {
        var act = () => PhotoType.From("invalid");
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyType_ShouldThrow(string? value)
    {
        var act = () => PhotoType.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void StaticInstances_ShouldHaveCorrectValues()
    {
        PhotoType.Before.Value.Should().Be("before");
        PhotoType.After.Value.Should().Be("after");
        PhotoType.Detail.Value.Should().Be("detail");
        PhotoType.Overview.Value.Should().Be("overview");
        PhotoType.Document.Value.Should().Be("document");
        PhotoType.Other.Value.Should().Be("other");
    }
}
