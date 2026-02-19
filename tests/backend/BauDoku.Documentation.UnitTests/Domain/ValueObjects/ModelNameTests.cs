using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class ModelNameTests
{
    [Fact]
    public void From_WithValidName_ShouldSucceed()
    {
        var model = ModelName.From("VZ312N");

        model.Value.Should().Be("VZ312N");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyName_ShouldThrow(string? value)
    {
        var act = () => ModelName.From(value!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongName_ShouldThrow()
    {
        var longName = new string('a', ModelName.MaxLength + 1);

        var act = () => ModelName.From(longName);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithMaxLengthName_ShouldSucceed()
    {
        var maxName = new string('a', ModelName.MaxLength);

        var model = ModelName.From(maxName);

        model.Value.Should().HaveLength(ModelName.MaxLength);
    }
}
