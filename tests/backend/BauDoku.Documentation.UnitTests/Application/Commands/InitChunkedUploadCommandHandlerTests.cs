using AwesomeAssertions;
using BauDoku.Documentation.Application.Commands.InitChunkedUpload;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain;
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
            GpsPosition.Create(Latitude.From(48.137154), Longitude.From(11.576124), null, HorizontalAccuracy.From(3.5), GpsSource.From("gps")));

    private static InitChunkedUploadCommand CreateValidCommand(InstallationIdentifier installationId) =>
        new(installationId, FileName.From("photo.jpg"), ContentType.From("image/jpeg"), FileSize.From(5 * 1024 * 1024), 5,
            PhotoType.Before, null, null, null, null, null, null, null);

    [Fact]
    public async Task Handle_WithValidCommand_ShouldInitSession()
    {
        var installation = CreateValidInstallation();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);
        chunkedUploadStorage.InitSessionAsync(Arg.Any<ChunkedUploadSession>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => UploadSessionIdentifier.From(callInfo.Arg<ChunkedUploadSession>().SessionId));

        var command = CreateValidCommand(installation.Id);

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

        var command = CreateValidCommand(InstallationIdentifier.New());

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
