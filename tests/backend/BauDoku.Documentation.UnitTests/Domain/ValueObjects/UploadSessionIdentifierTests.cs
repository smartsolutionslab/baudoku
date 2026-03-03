using AwesomeAssertions;
using SmartSolutionsLab.BauDoku.Documentation.Domain;

namespace SmartSolutionsLab.BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class UploadSessionIdentifierTests
{
    [Fact]
    public void From_WithValidGuid_ShouldSucceed()
    {
        var guid = Guid.NewGuid();
        var id = UploadSessionIdentifier.From(guid);
        id.Value.Should().Be(guid);
    }

    [Fact]
    public void From_WithEmptyGuid_ShouldThrow()
    {
        var act = () => UploadSessionIdentifier.From(Guid.Empty);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void New_ShouldGenerateUniqueIds()
    {
        var id1 = UploadSessionIdentifier.New();
        var id2 = UploadSessionIdentifier.New();
        id1.Should().NotBe(id2);
    }
}
