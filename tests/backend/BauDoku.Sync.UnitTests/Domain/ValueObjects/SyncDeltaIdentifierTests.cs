using AwesomeAssertions;
using BauDoku.Sync.Domain;

namespace BauDoku.Sync.UnitTests.Domain.ValueObjects;

public sealed class SyncDeltaIdentifierTests
{
    [Fact]
    public void From_WithValidGuid_ShouldSucceed()
    {
        var guid = Guid.NewGuid();
        var id = SyncDeltaIdentifier.From(guid);

        id.Value.Should().Be(guid);
    }

    [Fact]
    public void From_WithEmptyGuid_ShouldThrow()
    {
        var act = () => SyncDeltaIdentifier.From(Guid.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void New_ShouldCreateUniqueId()
    {
        var id1 = SyncDeltaIdentifier.New();
        var id2 = SyncDeltaIdentifier.New();

        id1.Value.Should().NotBe(Guid.Empty);
        id1.Should().NotBe(id2);
    }
}
