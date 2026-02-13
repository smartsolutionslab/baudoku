using AwesomeAssertions;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.BuildingBlocks.UnitTests.Guards;

public sealed class EnsureReferenceGuardTests
{
    [Fact]
    public void IsNotNull_WithValue_ShouldNotThrow()
    {
        Action act = () => Ensure.That(new object()).IsNotNull();
        act.Should().NotThrow();
    }

    [Fact]
    public void IsNotNull_WithNull_ShouldThrow()
    {
        Action act = () => Ensure.That((object?)null).IsNotNull();
        act.Should().Throw<ArgumentNullException>();
    }
}
