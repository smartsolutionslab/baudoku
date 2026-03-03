using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Sync.Domain;

namespace SmartSolutionsLab.BauDoku.Sync.UnitTests.Domain.ValueObjects;

public sealed class ConflictResolutionStrategyTests
{
    [Theory]
    [InlineData("client_wins")]
    [InlineData("server_wins")]
    [InlineData("manual_merge")]
    public void From_WithValidStrategy_ShouldSucceed(string value)
    {
        var strategy = ConflictResolutionStrategy.From(value);
        strategy.Value.Should().Be(value);
    }

    [Fact]
    public void From_WithInvalidStrategy_ShouldThrow()
    {
        var act = () => ConflictResolutionStrategy.From("invalid");
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => ConflictResolutionStrategy.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void StaticInstances_ShouldHaveCorrectValues()
    {
        ConflictResolutionStrategy.ClientWins.Value.Should().Be("client_wins");
        ConflictResolutionStrategy.ServerWins.Value.Should().Be("server_wins");
        ConflictResolutionStrategy.ManualMerge.Value.Should().Be("manual_merge");
    }
}
