using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class InstallationIdentifierTests
{
    [Fact]
    public void Create_WithValidGuid_ShouldSucceed()
    {
        var guid = Guid.NewGuid();
        var id = InstallationIdentifier.From(guid);
        id.Value.Should().Be(guid);
    }

    [Fact]
    public void Create_WithEmptyGuid_ShouldThrow()
    {
        var act = () => InstallationIdentifier.From(Guid.Empty);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void New_ShouldGenerateUniqueIds()
    {
        var id1 = InstallationIdentifier.New();
        var id2 = InstallationIdentifier.New();
        id1.Should().NotBe(id2);
    }
}
