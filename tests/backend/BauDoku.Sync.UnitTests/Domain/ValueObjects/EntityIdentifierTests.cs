using AwesomeAssertions;
using BauDoku.Sync.Domain;

namespace BauDoku.Sync.UnitTests.Domain.ValueObjects;

public sealed class EntityIdentifierTests
{
    [Fact]
    public void From_WithValidGuid_ShouldSucceed()
    {
        var guid = Guid.NewGuid();
        var id = EntityIdentifier.From(guid);
        id.Value.Should().Be(guid);
    }

    [Fact]
    public void From_WithEmptyGuid_ShouldThrow()
    {
        var act = () => EntityIdentifier.From(Guid.Empty);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void New_ShouldGenerateUniqueIds()
    {
        var id1 = EntityIdentifier.New();
        var id2 = EntityIdentifier.New();
        id1.Should().NotBe(id2);
    }
}
