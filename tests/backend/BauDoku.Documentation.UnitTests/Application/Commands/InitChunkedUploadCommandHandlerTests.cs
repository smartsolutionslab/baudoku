using AwesomeAssertions;
using BauDoku.Documentation.Application.Commands.InitChunkedUpload;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class InitChunkedUploadCommandHandlerTests
{
    private readonly IInstallationRepository installations;
    private readonly IChunkedUploadStorage chunkedUploadStorage;
    private readonly InitChunkedUploadCommandHandler handler;

    public InitChunkedUploadCommandHandlerTests()
    {
        installations = Substitute.For<IInstallationRepository>();
        chunkedUploadStorage = Substitute.For<IChunkedUploadStorage>();
        handler = new InitChunkedUploadCommandHandler(installations, chunkedUploadStorage);
    }

    private static Installation CreateValidInstallation() =>
        Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.CableTray,
            GpsPosition.Create(48.137154, 11.576124, null, 3.5, "gps"));

    private static InitChunkedUploadCommand CreateValidCommand(Guid installationId) =>
        new(installationId, "photo.jpg", "image/jpeg", 5 * 1024 * 1024, 5,
            "before", null, null, null, null, null, null, null);

    [Fact]
    public async Task Handle_WithValidCommand_ShouldInitSession()
    {
        var installation = CreateValidInstallation();
        var expectedSessionId = Guid.NewGuid();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);
        chunkedUploadStorage.InitSessionAsync(Arg.Any<ChunkedUploadSession>(), Arg.Any<CancellationToken>())
            .Returns(expectedSessionId);

        var command = CreateValidCommand(installation.Id.Value);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
        await chunkedUploadStorage.Received(1)
            .InitSessionAsync(Arg.Is<ChunkedUploadSession>(s =>
                s.InstallationId == installation.Id.Value &&
                s.FileName == "photo.jpg" &&
                s.TotalChunks == 5), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInstallationNotFound_ShouldThrow()
    {
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Throws(new KeyNotFoundException());

        var command = CreateValidCommand(Guid.NewGuid());

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
