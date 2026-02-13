using BauDoku.Documentation.Application.Commands.UploadChunk;
using FluentValidation.TestHelper;

namespace BauDoku.Documentation.UnitTests.Application.Validators;

public sealed class UploadChunkCommandValidatorTests
{
    private readonly UploadChunkCommandValidator _validator = new();

    private static UploadChunkCommand CreateValidCommand() =>
        new(Guid.NewGuid(), 0, new MemoryStream([1, 2, 3]));

    [Fact]
    public void ValidCommand_ShouldHaveNoErrors()
    {
        var result = _validator.TestValidate(CreateValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void SessionId_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { SessionId = Guid.Empty };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.SessionId);
    }

    [Fact]
    public void ChunkIndex_WhenNegative_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { ChunkIndex = -1 };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.ChunkIndex);
    }

    [Fact]
    public void Data_WhenNull_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Data = null! };
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Data);
    }
}
