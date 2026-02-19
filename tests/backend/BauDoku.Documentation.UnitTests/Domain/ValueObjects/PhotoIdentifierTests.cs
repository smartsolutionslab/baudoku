using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class PhotoIdentifierTests
{
    [Fact]
    public void From_WithValidGuid_ShouldSucceed()
    {
        var guid = Guid.NewGuid();
        var id = PhotoIdentifier.From(guid);

        id.Value.Should().Be(guid);
    }

    [Fact]
    public void From_WithEmptyGuid_ShouldThrow()
    {
        var act = () => PhotoIdentifier.From(Guid.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void New_ShouldCreateUniqueId()
    {
        var id1 = PhotoIdentifier.New();
        var id2 = PhotoIdentifier.New();

        id1.Value.Should().NotBe(Guid.Empty);
        id1.Should().NotBe(id2);
    }
}
