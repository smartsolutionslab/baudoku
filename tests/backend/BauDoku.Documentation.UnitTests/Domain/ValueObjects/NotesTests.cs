using AwesomeAssertions;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.UnitTests.Domain.ValueObjects;

public sealed class NotesTests
{
    [Fact]
    public void From_WithValidNotes_ShouldSucceed()
    {
        var notes = Notes.From("Kabel unter Estrich verlegt");
        notes.Value.Should().Be("Kabel unter Estrich verlegt");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyValue_ShouldThrow(string? value)
    {
        var act = () => Notes.From(value!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithTooLongValue_ShouldThrow()
    {
        var longValue = new string('a', Notes.MaxLength + 1);
        var act = () => Notes.From(longValue);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FromNullable_WithNull_ShouldReturnNull()
    {
        Notes.FromNullable(null).Should().BeNull();
    }

    [Fact]
    public void FromNullable_WithValue_ShouldReturnInstance()
    {
        Notes.FromNullable("Notiz")!.Value.Should().Be("Notiz");
    }
}
