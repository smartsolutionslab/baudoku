using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class ProjectIdentifierTests
{
    [Fact]
    public void From_WithValidGuid_ShouldSucceed()
    {
        var guid = Guid.NewGuid();
        var id = ProjectIdentifier.From(guid);
        id.Value.Should().Be(guid);
    }

    [Fact]
    public void From_WithEmptyGuid_ShouldThrow()
    {
        var act = () => ProjectIdentifier.From(Guid.Empty);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void New_ShouldGenerateUniqueIds()
    {
        var id1 = ProjectIdentifier.New();
        var id2 = ProjectIdentifier.New();
        id1.Should().NotBe(id2);
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        ProjectIdentifier.FromNullable(null).Should().BeNull();
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnInstance()
    {
        var guid = Guid.NewGuid();
        ProjectIdentifier.FromNullable(guid)!.Value.Should().Be(guid);
    }
}
