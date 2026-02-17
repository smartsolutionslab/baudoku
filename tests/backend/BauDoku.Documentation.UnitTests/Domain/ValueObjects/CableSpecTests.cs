using AwesomeAssertions;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class CableSpecTests
{
    [Fact]
    public void Create_WithValidCableSpec_ShouldSucceed()
    {
        var spec = CableSpec.Create("NYM-J 5x2.5", 25, "grey", 5);
        spec.CableType.Value.Should().Be("NYM-J 5x2.5");
        spec.CrossSection!.Value.Should().Be(25);
        spec.Color!.Value.Should().Be("grey");
        spec.ConductorCount.Should().Be(5);
    }

    [Fact]
    public void Create_WithOnlyRequiredFields_ShouldSucceed()
    {
        var spec = CableSpec.Create("NYY-J 3x1.5");
        spec.CrossSection.Should().BeNull();
        spec.Color.Should().BeNull();
        spec.ConductorCount.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyCableType_ShouldThrow(string? cableType)
    {
        var act = () => CableSpec.Create(cableType!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithInvalidCrossSection_ShouldThrow()
    {
        var act = () => CableSpec.Create("NYM-J", crossSection: 0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
