using AwesomeAssertions;
using BauDoku.Sync.Domain.ValueObjects;

namespace BauDoku.Sync.UnitTests.Domain.ValueObjects;

public sealed class EntityReferenceTests
{
    [Fact]
    public void Create_WithValidValues_ShouldSucceed()
    {
        var entityId = Guid.NewGuid();
        var entityRef = new EntityReference(EntityType.Project, entityId);

        entityRef.EntityType.Should().Be(EntityType.Project);
        entityRef.EntityId.Should().Be(entityId);
    }

    [Fact]
    public void Create_WithNullEntityType_ShouldThrow()
    {
        var act = () => new EntityReference(null!, Guid.NewGuid());
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithEmptyEntityId_ShouldThrow()
    {
        var act = () => new EntityReference(EntityType.Project, Guid.Empty);
        act.Should().Throw<ArgumentException>();
    }
}
