using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class InstallationStatusTests
{
    [Theory]
    [InlineData("in_progress")]
    [InlineData("completed")]
    [InlineData("inspected")]
    public void Create_WithValidStatus_ShouldSucceed(string value)
    {
        var status = new InstallationStatus(value);
        status.Value.Should().Be(value);
    }

    [Fact]
    public void Create_WithInvalidStatus_ShouldThrow()
    {
        var act = () => new InstallationStatus("invalid");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void StaticInstances_ShouldHaveCorrectValues()
    {
        InstallationStatus.InProgress.Value.Should().Be("in_progress");
        InstallationStatus.Completed.Value.Should().Be("completed");
        InstallationStatus.Inspected.Value.Should().Be("inspected");
    }
}
