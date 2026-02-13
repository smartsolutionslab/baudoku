using AwesomeAssertions;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.UnitTests.Domain.ValueObjects;

public sealed class EnumValueObjectTests
{
    [Fact]
    public void DeltaOperation_StaticInstances_ShouldHaveCorrectValues()
    {
        DeltaOperation.Create.Value.Should().Be("create");
        DeltaOperation.Update.Value.Should().Be("update");
        DeltaOperation.Delete.Value.Should().Be("delete");
    }

    [Fact]
    public void DeltaOperation_InvalidValue_ShouldThrow()
    {
        var act = () => DeltaOperation.From("invalid");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void BatchStatus_StaticInstances_ShouldHaveCorrectValues()
    {
        BatchStatus.Pending.Value.Should().Be("pending");
        BatchStatus.Processing.Value.Should().Be("processing");
        BatchStatus.Completed.Value.Should().Be("completed");
        BatchStatus.PartialConflict.Value.Should().Be("partial_conflict");
        BatchStatus.Failed.Value.Should().Be("failed");
    }

    [Fact]
    public void BatchStatus_InvalidValue_ShouldThrow()
    {
        var act = () => BatchStatus.From("invalid");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ConflictStatus_StaticInstances_ShouldHaveCorrectValues()
    {
        ConflictStatus.Unresolved.Value.Should().Be("unresolved");
        ConflictStatus.ClientWins.Value.Should().Be("client_wins");
        ConflictStatus.ServerWins.Value.Should().Be("server_wins");
        ConflictStatus.Merged.Value.Should().Be("merged");
    }

    [Fact]
    public void ConflictStatus_InvalidValue_ShouldThrow()
    {
        var act = () => ConflictStatus.From("invalid");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ConflictResolutionStrategy_StaticInstances_ShouldHaveCorrectValues()
    {
        ConflictResolutionStrategy.ClientWins.Value.Should().Be("client_wins");
        ConflictResolutionStrategy.ServerWins.Value.Should().Be("server_wins");
        ConflictResolutionStrategy.ManualMerge.Value.Should().Be("manual_merge");
    }

    [Fact]
    public void ConflictResolutionStrategy_InvalidValue_ShouldThrow()
    {
        var act = () => ConflictResolutionStrategy.From("invalid");
        act.Should().Throw<ArgumentException>();
    }
}
