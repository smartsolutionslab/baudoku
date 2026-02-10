using AwesomeAssertions;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.UnitTests.Domain.ValueObjects;

public sealed class SyncBatchIdTests
{
    [Fact]
    public void Create_WithValidGuid_ShouldSucceed()
    {
        var guid = Guid.NewGuid();
        var id = new SyncBatchId(guid);
        id.Value.Should().Be(guid);
    }

    [Fact]
    public void Create_WithEmptyGuid_ShouldThrow()
    {
        var act = () => new SyncBatchId(Guid.Empty);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void New_ShouldCreateUniqueId()
    {
        var id1 = SyncBatchId.New();
        var id2 = SyncBatchId.New();
        id1.Should().NotBe(id2);
    }
}
