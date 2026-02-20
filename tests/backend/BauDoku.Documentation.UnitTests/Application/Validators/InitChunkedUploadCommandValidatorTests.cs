using BauDoku.Documentation.Application.Commands.InitChunkedUpload;
using BauDoku.Documentation.Domain;
using FluentValidation.TestHelper;

namespace BauDoku.Documentation.UnitTests.Application.Validators;

public sealed class InitChunkedUploadCommandValidatorTests
{
    private readonly InitChunkedUploadCommandValidator validator = new();

    private static InitChunkedUploadCommand CreateValidCommand() =>
        new(InstallationIdentifier.New(), FileName.From("photo.jpg"), ContentType.From("image/jpeg"), FileSize.From(5 * 1024 * 1024), 5,
            PhotoType.Before, null, null, null, null, null, null, null);

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
    public void TotalSize_WhenExceeds50MB_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { TotalSize = FileSize.From(50 * 1024 * 1024 + 1) };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.TotalSize);
    }

    [Fact]
    public void TotalChunks_WhenZero_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { TotalChunks = 0 };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.TotalChunks);
    }

    [Fact]
    public void TotalChunks_WhenExceeds50_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { TotalChunks = 51 };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.TotalChunks);
    }
}
