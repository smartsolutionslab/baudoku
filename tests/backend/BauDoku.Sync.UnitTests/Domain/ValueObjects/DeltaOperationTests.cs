using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Sync.Domain;

namespace SmartSolutionsLab.BauDoku.Sync.UnitTests.Domain.ValueObjects;

public sealed class DeltaOperationTests
{
    [Theory]
    [InlineData("create")]
    [InlineData("update")]
    [InlineData("delete")]
    public void From_WithValidOperation_ShouldSucceed(string value)
    {
        var op = DeltaOperation.From(value);
        op.Value.Should().Be(value);
    }

    [Fact]
    public void From_WithInvalidOperation_ShouldThrow()
    {
        var act = () => DeltaOperation.From("invalid");
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => DeltaOperation.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void StaticInstances_ShouldHaveCorrectValues()
    {
        DeltaOperation.Create.Value.Should().Be("create");
        DeltaOperation.Update.Value.Should().Be("update");
        DeltaOperation.Delete.Value.Should().Be("delete");
    }
}
