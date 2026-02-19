using AwesomeAssertions;
using BauDoku.Sync.Domain;

namespace BauDoku.Sync.UnitTests.Domain.ValueObjects;

public sealed class EntityTypeTests
{
    [Theory]
    [InlineData("project")]
    [InlineData("zone")]
    [InlineData("installation")]
    [InlineData("photo")]
    [InlineData("measurement")]
    public void Create_WithValidValue_ShouldSucceed(string value)
    {
        var type = EntityType.From(value);
        type.Value.Should().Be(value);
    }

    [Fact]
    public void Create_WithInvalidValue_ShouldThrow()
    {
        var act = () => EntityType.From("unknown");
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => EntityType.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void StaticInstances_ShouldHaveCorrectValues()
    {
        EntityType.Project.Value.Should().Be("project");
        EntityType.Zone.Value.Should().Be("zone");
        EntityType.Installation.Value.Should().Be("installation");
        EntityType.Photo.Value.Should().Be("photo");
        EntityType.Measurement.Value.Should().Be("measurement");
    }
}
