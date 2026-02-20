using BauDoku.Documentation.Application.Commands;
using BauDoku.Documentation.Application.Commands.Validators;
using BauDoku.Documentation.Domain;
using FluentValidation.TestHelper;

namespace BauDoku.Documentation.UnitTests.Application.Validators;

public sealed class AddPhotoCommandValidatorTests
{
    private readonly AddPhotoCommandValidator validator = new();

    private static AddPhotoCommand CreateValidCommand() =>
        new(InstallationIdentifier.New(), FileName.From("photo.jpg"), ContentType.From("image/jpeg"), FileSize.From(1024 * 100), PhotoType.Before,
            null, null, null, null, null, null, null, new MemoryStream([1, 2, 3]));

    [Fact]
    public void ValidCommand_ShouldHaveNoErrors()
    {
        var result = validator.TestValidate(CreateValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ContentType_WhenNotAllowed_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { ContentType = ContentType.From("image/gif") };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.ContentType);
    }

    [Theory]
    [InlineData("image/jpeg")]
    [InlineData("image/png")]
    [InlineData("image/heic")]
    public void ContentType_WhenAllowed_ShouldNotHaveError(string contentType)
    {
        var cmd = CreateValidCommand() with { ContentType = ContentType.From(contentType) };
        validator.TestValidate(cmd).ShouldNotHaveValidationErrorFor(x => x.ContentType);
    }

    [Fact]
    public void FileSize_WhenExceeds50MB_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { FileSize = FileSize.From(50 * 1024 * 1024 + 1) };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.FileSize);
    }

    [Fact]
    public void Stream_WhenNull_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Stream = null! };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Stream);
    }
}
