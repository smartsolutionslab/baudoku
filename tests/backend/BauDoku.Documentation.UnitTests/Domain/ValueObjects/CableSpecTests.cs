using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class CableSpecTests
{
    [Fact]
    public void Create_WithValidCableSpec_ShouldSucceed()
    {
        var spec = CableSpec.Create(CableType.From("NYM-J 5x2.5"), CrossSection.From(25), CableColor.From("grey"), ConductorCount.From(5));
        spec.CableType.Value.Should().Be("NYM-J 5x2.5");
        spec.CrossSection!.Value.Should().Be(25);
        spec.Color!.Value.Should().Be("grey");
        spec.ConductorCount!.Value.Should().Be(5);
    }

    [Fact]
    public void Create_WithOnlyRequiredFields_ShouldSucceed()
    {
        var spec = CableSpec.Create(CableType.From("NYY-J 3x1.5"));
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
        var act = () => CableType.From(cableType!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithInvalidCrossSection_ShouldThrow()
    {
        var act = () => CrossSection.From(0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void FromNullable_WithNullCableType_ShouldReturnNull()
    {
        var result = CableSpec.FromNullable(null, CrossSection.From(2.5m), CableColor.From("rot"));
        result.Should().BeNull();
    }

    [Fact]
    public void FromNullable_WithCableType_ShouldReturnInstance()
    {
        var result = CableSpec.FromNullable(CableType.From("NYM-J 5x2.5"), CrossSection.From(25));
        result.Should().NotBeNull();
        result!.CableType.Value.Should().Be("NYM-J 5x2.5");
        result.CrossSection!.Value.Should().Be(25);
    }
}
