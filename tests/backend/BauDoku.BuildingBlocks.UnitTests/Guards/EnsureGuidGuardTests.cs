using AwesomeAssertions;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.BuildingBlocks.UnitTests.Guards;

public sealed class EnsureGuidGuardTests
{
    [Fact]
    public void IsNotEmpty_WithValidGuid_ShouldNotThrow()
    {
        Action act = () => Ensure.That(Guid.NewGuid()).IsNotEmpty();
        act.Should().NotThrow();
    }

    [Fact]
    public void IsNotEmpty_WithEmptyGuid_ShouldThrow()
    {
        Action act = () => Ensure.That(Guid.Empty).IsNotEmpty();
        act.Should().Throw<ArgumentException>();
    }
}
