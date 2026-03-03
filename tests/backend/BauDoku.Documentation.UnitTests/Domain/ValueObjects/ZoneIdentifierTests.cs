using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class ZoneIdentifierTests
{
    [Fact]
    public void From_WithValidGuid_ShouldSucceed()
    {
        var guid = Guid.NewGuid();
        var id = ZoneIdentifier.From(guid);
        id.Value.Should().Be(guid);
    }

    [Fact]
    public void From_WithEmptyGuid_ShouldThrow()
    {
        var act = () => ZoneIdentifier.From(Guid.Empty);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void New_ShouldGenerateUniqueIds()
    {
        var id1 = ZoneIdentifier.New();
        var id2 = ZoneIdentifier.New();
        id1.Should().NotBe(id2);
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        ZoneIdentifier.FromNullable(null).Should().BeNull();
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnInstance()
    {
        var guid = Guid.NewGuid();
        ZoneIdentifier.FromNullable(guid)!.Value.Should().Be(guid);
    }
}
