using AwesomeAssertions;
using BauDoku.Documentation.Application.Commands;
using BauDoku.Documentation.Application.Commands.Handlers;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class CompleteChunkedUploadCommandHandlerTests
{
    private readonly IChunkedUploadStorage chunkedUploadStorage;
    private readonly IPhotoStorage photoStorage;
    private readonly IInstallationRepository installations;
    private readonly CompleteChunkedUploadCommandHandler handler;

    public CompleteChunkedUploadCommandHandlerTests()
    {
        chunkedUploadStorage = Substitute.For<IChunkedUploadStorage>();
        photoStorage = Substitute.For<IPhotoStorage>();
        installations = Substitute.For<IInstallationRepository>();
        handler = new CompleteChunkedUploadCommandHandler(chunkedUploadStorage, photoStorage, installations);
    }

    private static Installation CreateValidInstallation() =>
        Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.CableTray,
            GpsPosition.Create(
                Latitude.From(48.137154),
                Longitude.From(11.576124),
                null,
                HorizontalAccuracy.From(3.5),
                GpsSource.From("gps")
            ));

    private static ChunkedUploadSession CreateValidSession(Guid sessionId, Guid installationId) =>
        new(sessionId, installationId, "photo.jpg", "image/jpeg",
            5 * 1024 * 1024, 5, "before", null, null, null, null, null, null, null,
            DateTime.UtcNow);

    [Fact]
    public async Task Handle_WithAllChunksUploaded_ShouldAssembleAndAddPhoto()
    {
        var installation = CreateValidInstallation();
        var sessionId = Guid.NewGuid();
        var sessionIdentifier = UploadSessionIdentifier.From(sessionId);
        var session = CreateValidSession(sessionId, installation.Id.Value);

        chunkedUploadStorage.GetSessionAsync(sessionIdentifier, Arg.Any<CancellationToken>())
            .Returns(session);
        chunkedUploadStorage.GetUploadedChunkCountAsync(sessionIdentifier, Arg.Any<CancellationToken>())
            .Returns(5);
        chunkedUploadStorage.AssembleAsync(sessionIdentifier, Arg.Any<CancellationToken>())
            .Returns(new MemoryStream([1, 2, 3]));
        photoStorage.UploadAsync(Arg.Any<Stream>(), Arg.Any<FileName>(), Arg.Any<ContentType>(), Arg.Any<CancellationToken>())
            .Returns(BlobUrl.From("https://blob.storage/photo.jpg"));
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new CompleteChunkedUploadCommand(sessionIdentifier);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Value.Should().NotBe(Guid.Empty);
        installation.Photos.Should().ContainSingle();
        await photoStorage.Received(1).UploadAsync(Arg.Any<Stream>(), FileName.From("photo.jpg"), ContentType.From("image/jpeg"), Arg.Any<CancellationToken>());
        await installations.Received(1).SaveAsync(Arg.Any<Installation>(), Arg.Any<CancellationToken>());
        await chunkedUploadStorage.Received(1).CleanupSessionAsync(sessionIdentifier, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSessionNotFound_ShouldThrow()
    {
        chunkedUploadStorage.GetSessionAsync(Arg.Any<UploadSessionIdentifier>(), Arg.Any<CancellationToken>())
            .Throws(new KeyNotFoundException("Upload-Session nicht gefunden."));

        var command = new CompleteChunkedUploadCommand(UploadSessionIdentifier.New());

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenChunksIncomplete_ShouldThrow()
    {
        var sessionId = Guid.NewGuid();
        var sessionIdentifier = UploadSessionIdentifier.From(sessionId);
        var session = CreateValidSession(sessionId, Guid.NewGuid());

        chunkedUploadStorage.GetSessionAsync(sessionIdentifier, Arg.Any<CancellationToken>())
            .Returns(session);
        chunkedUploadStorage.GetUploadedChunkCountAsync(sessionIdentifier, Arg.Any<CancellationToken>())
            .Returns(3);

        var command = new CompleteChunkedUploadCommand(sessionIdentifier);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*3/5*");
    }

    [Fact]
    public async Task Handle_WithGpsPosition_ShouldSetPhotoPosition()
    {
        var installation = CreateValidInstallation();
        var sessionId = Guid.NewGuid();
        var sessionIdentifier = UploadSessionIdentifier.From(sessionId);
        var session = new ChunkedUploadSession(
            sessionId, installation.Id.Value, "photo.jpg", "image/jpeg",
            5 * 1024 * 1024, 5, "before", null, null,
            48.0, 11.0, 500.0, 5.0, "gps", DateTime.UtcNow);

        chunkedUploadStorage.GetSessionAsync(sessionIdentifier, Arg.Any<CancellationToken>()).Returns(session);
        chunkedUploadStorage.GetUploadedChunkCountAsync(sessionIdentifier, Arg.Any<CancellationToken>()).Returns(5);
        chunkedUploadStorage.AssembleAsync(sessionIdentifier, Arg.Any<CancellationToken>()).Returns(new MemoryStream([1, 2, 3]));
        photoStorage.UploadAsync(Arg.Any<Stream>(), Arg.Any<FileName>(), Arg.Any<ContentType>(), Arg.Any<CancellationToken>())
            .Returns(BlobUrl.From("https://blob.storage/photo.jpg"));
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>()).Returns(installation);

        var command = new CompleteChunkedUploadCommand(sessionIdentifier);

        await handler.Handle(command, CancellationToken.None);

        installation.Photos.Should().ContainSingle();
        installation.Photos[0].Position.Should().NotBeNull();
        installation.Photos[0].Position!.Latitude.Value.Should().Be(48.0);
    }

    [Fact]
    public async Task Handle_WhenCleanupCalled_ShouldCleanupAfterSave()
    {
        var installation = CreateValidInstallation();
        var sessionId = Guid.NewGuid();
        var sessionIdentifier = UploadSessionIdentifier.From(sessionId);
        var session = CreateValidSession(sessionId, installation.Id.Value);

        chunkedUploadStorage.GetSessionAsync(sessionIdentifier, Arg.Any<CancellationToken>()).Returns(session);
        chunkedUploadStorage.GetUploadedChunkCountAsync(sessionIdentifier, Arg.Any<CancellationToken>()).Returns(5);
        chunkedUploadStorage.AssembleAsync(sessionIdentifier, Arg.Any<CancellationToken>()).Returns(new MemoryStream([1, 2, 3]));
        photoStorage.UploadAsync(Arg.Any<Stream>(), Arg.Any<FileName>(), Arg.Any<ContentType>(), Arg.Any<CancellationToken>())
            .Returns(BlobUrl.From("https://blob.storage/photo.jpg"));
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>()).Returns(installation);

        var command = new CompleteChunkedUploadCommand(sessionIdentifier);

        await handler.Handle(command, CancellationToken.None);

        Received.InOrder(() =>
        {
            installations.SaveAsync(Arg.Any<Installation>(), Arg.Any<CancellationToken>());
            chunkedUploadStorage.CleanupSessionAsync(sessionIdentifier, Arg.Any<CancellationToken>());
        });
    }
}
