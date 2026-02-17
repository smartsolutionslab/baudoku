using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Commands.CompleteChunkedUpload;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class CompleteChunkedUploadCommandHandlerTests
{
    private readonly IChunkedUploadStorage chunkedUploadStorage;
    private readonly IPhotoStorage photoStorage;
    private readonly IInstallationRepository installations;
    private readonly IUnitOfWork unitOfWork;
    private readonly CompleteChunkedUploadCommandHandler handler;

    public CompleteChunkedUploadCommandHandlerTests()
    {
        chunkedUploadStorage = Substitute.For<IChunkedUploadStorage>();
        photoStorage = Substitute.For<IPhotoStorage>();
        installations = Substitute.For<IInstallationRepository>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        handler = new CompleteChunkedUploadCommandHandler(
            chunkedUploadStorage, photoStorage, installations, unitOfWork);
    }

    private static Installation CreateValidInstallation() =>
        Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.CableTray,
            GpsPosition.Create(48.137154, 11.576124, null, 3.5, "gps"));

    private static ChunkedUploadSession CreateValidSession(Guid sessionId, Guid installationId) =>
        new(sessionId, installationId, "photo.jpg", "image/jpeg",
            5 * 1024 * 1024, 5, "before", null, null, null, null, null, null, null,
            DateTime.UtcNow);

    [Fact]
    public async Task Handle_WithAllChunksUploaded_ShouldAssembleAndAddPhoto()
    {
        var installation = CreateValidInstallation();
        var sessionId = Guid.NewGuid();
        var session = CreateValidSession(sessionId, installation.Id.Value);

        chunkedUploadStorage.GetSessionAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);
        chunkedUploadStorage.GetUploadedChunkCountAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(5);
        chunkedUploadStorage.AssembleAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(new MemoryStream([1, 2, 3]));
        photoStorage.UploadAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("https://blob.storage/photo.jpg");
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new CompleteChunkedUploadCommand(sessionId);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
        installation.Photos.Should().ContainSingle();
        await photoStorage.Received(1).UploadAsync(Arg.Any<Stream>(), "photo.jpg", "image/jpeg", Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await chunkedUploadStorage.Received(1).CleanupSessionAsync(sessionId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSessionNotFound_ShouldThrow()
    {
        chunkedUploadStorage.GetSessionAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((ChunkedUploadSession?)null);

        var command = new CompleteChunkedUploadCommand(Guid.NewGuid());

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenChunksIncomplete_ShouldThrow()
    {
        var sessionId = Guid.NewGuid();
        var session = CreateValidSession(sessionId, Guid.NewGuid());

        chunkedUploadStorage.GetSessionAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);
        chunkedUploadStorage.GetUploadedChunkCountAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(3);

        var command = new CompleteChunkedUploadCommand(sessionId);

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*3/5*");
    }

    [Fact]
    public async Task Handle_WithGpsPosition_ShouldSetPhotoPosition()
    {
        var installation = CreateValidInstallation();
        var sessionId = Guid.NewGuid();
        var session = new ChunkedUploadSession(
            sessionId, installation.Id.Value, "photo.jpg", "image/jpeg",
            5 * 1024 * 1024, 5, "before", null, null,
            48.0, 11.0, 500.0, 5.0, "gps", DateTime.UtcNow);

        chunkedUploadStorage.GetSessionAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);
        chunkedUploadStorage.GetUploadedChunkCountAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(5);
        chunkedUploadStorage.AssembleAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(new MemoryStream([1, 2, 3]));
        photoStorage.UploadAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("https://blob.storage/photo.jpg");
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new CompleteChunkedUploadCommand(sessionId);

        await handler.Handle(command, CancellationToken.None);

        installation.Photos.Should().ContainSingle();
        installation.Photos[0].Position.Should().NotBeNull();
        installation.Photos[0].Position!.Latitude.Should().Be(48.0);
    }

    [Fact]
    public async Task Handle_WhenCleanupCalled_ShouldCleanupAfterSave()
    {
        var installation = CreateValidInstallation();
        var sessionId = Guid.NewGuid();
        var session = CreateValidSession(sessionId, installation.Id.Value);

        chunkedUploadStorage.GetSessionAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);
        chunkedUploadStorage.GetUploadedChunkCountAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(5);
        chunkedUploadStorage.AssembleAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(new MemoryStream([1, 2, 3]));
        photoStorage.UploadAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("https://blob.storage/photo.jpg");
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new CompleteChunkedUploadCommand(sessionId);

        await handler.Handle(command, CancellationToken.None);

        Received.InOrder(() =>
        {
            unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>());
            chunkedUploadStorage.CleanupSessionAsync(sessionId, Arg.Any<CancellationToken>());
        });
    }
}
