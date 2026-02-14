using AwesomeAssertions;
using BauDoku.Projects.Domain.ValueObjects;

namespace BauDoku.Projects.UnitTests.Domain.ValueObjects;

public sealed class ProjectStatusTests
{
    [Theory]
    [InlineData("draft")]
    [InlineData("active")]
    [InlineData("completed")]
    [InlineData("archived")]
    public void From_WithValidStatus_ShouldSucceed(string value)
    {
        var status = ProjectStatus.From(value);

        status.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyStatus_ShouldThrow(string? value)
    {
        var act = () => ProjectStatus.From(value!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithInvalidStatus_ShouldThrow()
    {
        var act = () => ProjectStatus.From("invalid");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void StaticInstances_ShouldHaveCorrectValues()
    {
        ProjectStatus.Draft.Value.Should().Be("draft");
        ProjectStatus.Active.Value.Should().Be("active");
        ProjectStatus.Completed.Value.Should().Be("completed");
        ProjectStatus.Archived.Value.Should().Be("archived");
    }
}
