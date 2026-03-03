using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class FileSizeTests
{
    [Fact]
    public void From_WithValidSize_ShouldSucceed()
    {
        var size = FileSize.From(1024);
        size.Value.Should().Be(1024);
    }

    [Fact]
    public void From_WithZero_ShouldThrow()
    {
        var act = () => FileSize.From(0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void From_WithNegative_ShouldThrow()
    {
        var act = () => FileSize.From(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
