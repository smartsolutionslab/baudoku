using AwesomeAssertions;
using BauDoku.Sync.Domain;

namespace BauDoku.Sync.UnitTests.Domain.ValueObjects;

public sealed class SyncLimitTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(500)]
    [InlineData(1000)]
    public void From_WithValidValue_ShouldCreateSyncLimit(int value)
    {
        var limit = SyncLimit.From(value);

        limit.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1001)]
    [InlineData(5000)]
    public void From_WithOutOfRangeValue_ShouldThrow(int value)
    {
        Action act = () => SyncLimit.From(value);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Default_ShouldBeHundred()
    {
        SyncLimit.Default.Value.Should().Be(100);
    }

    [Fact]
    public void Max_ShouldBeThousand()
    {
        SyncLimit.Max.Should().Be(1000);
    }

    [Fact]
    public void FromNullable_WithValue_ShouldCreateSyncLimit()
    {
        var limit = SyncLimit.FromNullable(50);

        limit.Value.Should().Be(50);
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnDefault()
    {
        var limit = SyncLimit.FromNullable(null);

        limit.Should().Be(SyncLimit.Default);
    }
}
