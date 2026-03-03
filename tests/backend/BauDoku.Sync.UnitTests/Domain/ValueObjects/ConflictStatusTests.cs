using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Sync.Domain;

namespace SmartSolutionsLab.BauDoku.Sync.UnitTests.Domain.ValueObjects;

public sealed class ConflictStatusTests
{
    [Theory]
    [InlineData("unresolved")]
    [InlineData("client_wins")]
    [InlineData("server_wins")]
    [InlineData("merged")]
    public void From_WithValidStatus_ShouldSucceed(string value)
    {
        var status = ConflictStatus.From(value);
        status.Value.Should().Be(value);
    }

    [Fact]
    public void From_WithInvalidStatus_ShouldThrow()
    {
        var act = () => ConflictStatus.From("invalid");
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => ConflictStatus.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void StaticInstances_ShouldHaveCorrectValues()
    {
        ConflictStatus.Unresolved.Value.Should().Be("unresolved");
        ConflictStatus.ClientWins.Value.Should().Be("client_wins");
        ConflictStatus.ServerWins.Value.Should().Be("server_wins");
        ConflictStatus.Merged.Value.Should().Be("merged");
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        ConflictStatus.FromNullable(null).Should().BeNull();
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnInstance()
    {
        ConflictStatus.FromNullable("unresolved")!.Value.Should().Be("unresolved");
    }
}
