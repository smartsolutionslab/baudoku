using BauDoku.Sync.Application.Commands.ProcessSyncBatch;
using BauDoku.Sync.Application.Queries.Dtos;
using BauDoku.Sync.Domain.ValueObjects;
using FluentValidation.TestHelper;

namespace BauDoku.Sync.UnitTests.Application.Validators;

public sealed class ProcessSyncBatchCommandValidatorTests
{
    private readonly ProcessSyncBatchCommandValidator validator = new();

    private static SyncDeltaDto CreateValidDelta() =>
        new("project", Guid.NewGuid(), "create", 0, """{"name":"Test"}""", DateTime.UtcNow);

    private static ProcessSyncBatchCommand CreateValidCommand() =>
        new("device-001", [CreateValidDelta()]);

    [Fact]
    public void ValidCommand_ShouldHaveNoErrors()
    {
        var result = validator.TestValidate(CreateValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void DeviceId_WhenEmpty_ShouldHaveError(string? deviceId)
    {
        var cmd = CreateValidCommand() with { DeviceId = deviceId! };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.DeviceId);
    }

    [Fact]
    public void DeviceId_WhenTooLong_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { DeviceId = new string('a', DeviceIdentifier.MaxLength + 1) };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.DeviceId);
    }

    [Fact]
    public void Deltas_WhenEmpty_ShouldHaveError()
    {
        var cmd = CreateValidCommand() with { Deltas = [] };
        validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Deltas);
    }

    [Fact]
    public void Delta_EntityType_WhenEmpty_ShouldHaveError()
    {
        var delta = CreateValidDelta() with { EntityType = "" };
        var cmd = CreateValidCommand() with { Deltas = [delta] };
        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor("Deltas[0].EntityType");
    }

    [Fact]
    public void Delta_EntityId_WhenEmpty_ShouldHaveError()
    {
        var delta = CreateValidDelta() with { EntityId = Guid.Empty };
        var cmd = CreateValidCommand() with { Deltas = [delta] };
        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor("Deltas[0].EntityId");
    }

    [Fact]
    public void Delta_Operation_WhenEmpty_ShouldHaveError()
    {
        var delta = CreateValidDelta() with { Operation = "" };
        var cmd = CreateValidCommand() with { Deltas = [delta] };
        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor("Deltas[0].Operation");
    }

    [Fact]
    public void Delta_BaseVersion_WhenNegative_ShouldHaveError()
    {
        var delta = CreateValidDelta() with { BaseVersion = -1 };
        var cmd = CreateValidCommand() with { Deltas = [delta] };
        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor("Deltas[0].BaseVersion");
    }

    [Fact]
    public void Delta_Payload_WhenEmpty_ShouldHaveError()
    {
        var delta = CreateValidDelta() with { Payload = "" };
        var cmd = CreateValidCommand() with { Deltas = [delta] };
        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor("Deltas[0].Payload");
    }

    [Fact]
    public void Delta_Payload_WhenTooLong_ShouldHaveError()
    {
        var delta = CreateValidDelta() with { Payload = new string('x', DeltaPayload.MaxLength + 1) };
        var cmd = CreateValidCommand() with { Deltas = [delta] };
        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor("Deltas[0].Payload");
    }
}
