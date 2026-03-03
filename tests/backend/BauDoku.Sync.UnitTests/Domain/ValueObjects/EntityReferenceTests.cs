using AwesomeAssertions;
using BauDoku.Sync.Domain;

namespace BauDoku.Sync.UnitTests.Domain.ValueObjects;

public sealed class EntityReferenceTests
{
    [Fact]
    public void Create_WithValidValues_ShouldSucceed()
    {
        var entityId = EntityIdentifier.From(Guid.NewGuid());
        var entityRef = EntityReference.Create(EntityType.Project, entityId);

        entityRef.EntityType.Should().Be(EntityType.Project);
        entityRef.EntityId.Should().Be(entityId);
    }

    [Fact]
    public void Create_WithNullEntityType_ShouldThrow()
    {
        var act = () => EntityReference.Create(null!, EntityIdentifier.New());
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithNullEntityId_ShouldThrow()
    {
        var act = () => EntityReference.Create(EntityType.Project, null!);
        act.Should().Throw<ArgumentNullException>();
    }
}
