using AwesomeAssertions;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.UnitTests.Domain.ValueObjects;

public sealed class SyncVersionTests
{
    [Fact]
    public void Create_WithValidValue_ShouldSucceed()
    {
        var version = new SyncVersion(5);
        version.Value.Should().Be(5);
    }

    [Fact]
    public void Create_WithZero_ShouldSucceed()
    {
        var version = new SyncVersion(0);
        version.Value.Should().Be(0);
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldThrow()
    {
        var act = () => new SyncVersion(-1);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Initial_ShouldBeZero()
    {
        SyncVersion.Initial.Value.Should().Be(0);
    }

    [Fact]
    public void Increment_ShouldReturnNextVersion()
    {
        var version = new SyncVersion(3);
        var next = version.Increment();
        next.Value.Should().Be(4);
    }
}
