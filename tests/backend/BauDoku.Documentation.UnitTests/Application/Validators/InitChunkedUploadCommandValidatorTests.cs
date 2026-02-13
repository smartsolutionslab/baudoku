using BauDoku.Documentation.Application.Commands.InitChunkedUpload;
using BauDoku.Documentation.Domain.ValueObjects;
using FluentValidation.TestHelper;

namespace BauDoku.Documentation.UnitTests.Application.Validators;

public sealed class InitChunkedUploadCommandValidatorTests
{
    private readonly InitChunkedUploadCommandValidator validator = new();

    private static InitChunkedUploadCommand CreateValidCommand() =>
        new(Guid.NewGuid(), "photo.jpg", "image/jpeg", 5 * 1024 * 1024, 5,
            "before", null, null, null, null, null, null, null);

    [Fact]
    public void ValidCommand_ShouldHaveNoErrors()
    {
        var result = validator.TestValidate(CreateValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void InstallationId_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { InstallationId = Guid.Empty };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.InstallationId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void FileName_WhenEmpty_ShouldHaveError(string? fileName)
    {
        var cmd = CreateValidCommand() with { FileName = fileName! };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.FileName);
    }

    [Fact]
    public void FileName_WhenTooLong_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { FileName = new string('a', 256) };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.FileName);
    }

    [Fact]
    public void ContentType_WhenNotAllowed_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { ContentType = "image/gif" };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.ContentType);
    }

    [Theory]
    [InlineData("image/jpeg")]
    [InlineData("image/png")]
    [InlineData("image/heic")]
    public void ContentType_WhenAllowed_ShouldNotHaveError(string contentType)
    {
        var cmd = CreateValidCommand() with { ContentType = contentType };
        validator.TestValidate(cmd).ShouldNotHaveValidationErrorFor(x => x.ContentType);
    }

    [Fact]
    public void TotalSize_WhenZero_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { TotalSize = 0 };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.TotalSize);
    }

    [Fact]
    public void TotalSize_WhenExceeds50MB_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { TotalSize = 50 * 1024 * 1024 + 1 };
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

    [Fact]
    public void PhotoType_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { PhotoType = "" };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.PhotoType);
    }

    [Fact]
    public void Caption_WhenTooLong_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Caption = new string('a', Caption.MaxLength + 1) };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Caption);
    }
}
