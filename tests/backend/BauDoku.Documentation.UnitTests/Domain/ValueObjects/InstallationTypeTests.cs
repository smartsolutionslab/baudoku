using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class InstallationTypeTests
{
    [Theory]
    [InlineData("cable_tray")]
    [InlineData("junction_box")]
    [InlineData("cable_pull")]
    [InlineData("conduit")]
    [InlineData("grounding")]
    [InlineData("lightning_protection")]
    [InlineData("switchgear")]
    [InlineData("transformer")]
    [InlineData("other")]
    public void Create_WithValidType_ShouldSucceed(string value)
    {
        var type = new InstallationType(value);
        type.Value.Should().Be(value);
    }

    [Fact]
    public void Create_WithInvalidType_ShouldThrow()
    {
        var act = () => new InstallationType("invalid");
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyType_ShouldThrow(string? value)
    {
        var act = () => new InstallationType(value!);
        act.Should().Throw<ArgumentException>();
    }
}
