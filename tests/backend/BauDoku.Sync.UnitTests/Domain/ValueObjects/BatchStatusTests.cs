using AwesomeAssertions;
using BauDoku.Sync.Domain;

namespace BauDoku.Sync.UnitTests.Domain.ValueObjects;

public sealed class BatchStatusTests
{
    [Theory]
    [InlineData("pending")]
    [InlineData("processing")]
    [InlineData("completed")]
    [InlineData("partial_conflict")]
    [InlineData("failed")]
    public void From_WithValidStatus_ShouldSucceed(string value)
    {
        var status = BatchStatus.From(value);
        status.Value.Should().Be(value);
    }

    [Fact]
    public void From_WithInvalidStatus_ShouldThrow()
    {
        var act = () => BatchStatus.From("invalid");
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => BatchStatus.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void StaticInstances_ShouldHaveCorrectValues()
    {
        BatchStatus.Pending.Value.Should().Be("pending");
        BatchStatus.Processing.Value.Should().Be("processing");
        BatchStatus.Completed.Value.Should().Be("completed");
        BatchStatus.PartialConflict.Value.Should().Be("partial_conflict");
        BatchStatus.Failed.Value.Should().Be("failed");
    }
}
