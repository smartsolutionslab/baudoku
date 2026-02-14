using AwesomeAssertions;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.UnitTests.Domain.ValueObjects;

public sealed class ZoneIdentifierTests
{
    [Fact]
    public void From_WithValidGuid_ShouldSucceed()
    {
        var guid = Guid.NewGuid();
        var id = ZoneIdentifier.From(guid);

        id.Value.Should().Be(guid);
    }

    [Fact]
    public void From_WithEmptyGuid_ShouldThrow()
    {
        var act = () => ZoneIdentifier.From(Guid.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void New_ShouldCreateUniqueId()
    {
        var id1 = ZoneIdentifier.New();
        var id2 = ZoneIdentifier.New();

        id1.Value.Should().NotBe(Guid.Empty);
        id1.Should().NotBe(id2);
    }
}
