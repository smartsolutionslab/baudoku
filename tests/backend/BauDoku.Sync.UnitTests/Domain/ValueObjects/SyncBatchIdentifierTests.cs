using AwesomeAssertions;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.UnitTests.Domain.ValueObjects;

public sealed class SyncBatchIdentifierTests
{
    [Fact]
    public void Create_WithValidGuid_ShouldSucceed()
    {
        var guid = Guid.NewGuid();
        var id = SyncBatchIdentifier.From(guid);
        id.Value.Should().Be(guid);
    }

    [Fact]
    public void Create_WithEmptyGuid_ShouldThrow()
    {
        var act = () => SyncBatchIdentifier.From(Guid.Empty);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void New_ShouldCreateUniqueId()
    {
        var id1 = SyncBatchIdentifier.New();
        var id2 = SyncBatchIdentifier.New();
        id1.Should().NotBe(id2);
    }
}
