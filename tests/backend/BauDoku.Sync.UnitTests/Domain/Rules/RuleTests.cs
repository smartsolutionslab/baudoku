using AwesomeAssertions;
using BauDoku.Sync.Domain.Rules;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.UnitTests.Domain.Rules;

public sealed class RuleTests
{
    [Theory]
    [InlineData("completed")]
    [InlineData("failed")]
    [InlineData("partial_conflict")]
    public void BatchMustNotBeAlreadyProcessed_WhenProcessed_ShouldBeBroken(string status)
    {
        var rule = new BatchMustNotBeAlreadyProcessed(BatchStatus.From(status));
        rule.IsBroken().Should().BeTrue();
    }

    [Theory]
    [InlineData("pending")]
    [InlineData("processing")]
    public void BatchMustNotBeAlreadyProcessed_WhenNotProcessed_ShouldNotBeBroken(string status)
    {
        var rule = new BatchMustNotBeAlreadyProcessed(BatchStatus.From(status));
        rule.IsBroken().Should().BeFalse();
    }

    [Fact]
    public void ConflictMustBeUnresolved_WhenUnresolved_ShouldNotBeBroken()
    {
        var rule = new ConflictMustBeUnresolved(ConflictStatus.Unresolved);
        rule.IsBroken().Should().BeFalse();
    }

    [Theory]
    [InlineData("client_wins")]
    [InlineData("server_wins")]
    [InlineData("merged")]
    public void ConflictMustBeUnresolved_WhenResolved_ShouldBeBroken(string status)
    {
        var rule = new ConflictMustBeUnresolved(ConflictStatus.From(status));
        rule.IsBroken().Should().BeTrue();
    }
}
