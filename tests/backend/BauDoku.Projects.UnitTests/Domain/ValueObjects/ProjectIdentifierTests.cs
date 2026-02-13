using AwesomeAssertions;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.UnitTests.Domain.ValueObjects;

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
    public void New_ShouldCreateUniqueId()
    {
        var id1 = ProjectIdentifier.New();
        var id2 = ProjectIdentifier.New();

        id1.Value.Should().NotBe(Guid.Empty);
        id1.Should().NotBe(id2);
    }
}
