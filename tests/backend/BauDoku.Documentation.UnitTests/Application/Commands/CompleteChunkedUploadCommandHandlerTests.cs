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
    private readonly IChunkedUploadStorage _chunkedUploadStorage;
    private readonly IPhotoStorage _photoStorage;
    private readonly IInstallationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CompleteChunkedUploadCommandHandler _handler;

    public CompleteChunkedUploadCommandHandlerTests()
    {
        _chunkedUploadStorage = Substitute.For<IChunkedUploadStorage>();
        _photoStorage = Substitute.For<IPhotoStorage>();
        _repository = Substitute.For<IInstallationRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new CompleteChunkedUploadCommandHandler(
            _chunkedUploadStorage, _photoStorage, _repository, _unitOfWork);
    }

    private static Installation CreateValidInstallation() =>
        Installation.Create(
            InstallationId.New(),
            Guid.NewGuid(),
            null,
            InstallationType.CableTray,
            new GpsPosition(48.137154, 11.576124, null, 3.5, "gps"));

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

        _chunkedUploadStorage.GetSessionAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);
        _chunkedUploadStorage.GetUploadedChunkCountAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(5);
        _chunkedUploadStorage.AssembleAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(new MemoryStream([1, 2, 3]));
        _photoStorage.UploadAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("https://blob.storage/photo.jpg");
        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new CompleteChunkedUploadCommand(sessionId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
        installation.Photos.Should().ContainSingle();
        await _photoStorage.Received(1).UploadAsync(Arg.Any<Stream>(), "photo.jpg", "image/jpeg", Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _chunkedUploadStorage.Received(1).CleanupSessionAsync(sessionId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSessionNotFound_ShouldThrow()
    {
        _chunkedUploadStorage.GetSessionAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((ChunkedUploadSession?)null);

        var command = new CompleteChunkedUploadCommand(Guid.NewGuid());

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_WhenChunksIncomplete_ShouldThrow()
    {
        var sessionId = Guid.NewGuid();
        var session = CreateValidSession(sessionId, Guid.NewGuid());

        _chunkedUploadStorage.GetSessionAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);
        _chunkedUploadStorage.GetUploadedChunkCountAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(3);

        var command = new CompleteChunkedUploadCommand(sessionId);

        var act = () => _handler.Handle(command, CancellationToken.None);

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

        _chunkedUploadStorage.GetSessionAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);
        _chunkedUploadStorage.GetUploadedChunkCountAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(5);
        _chunkedUploadStorage.AssembleAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(new MemoryStream([1, 2, 3]));
        _photoStorage.UploadAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("https://blob.storage/photo.jpg");
        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new CompleteChunkedUploadCommand(sessionId);

        await _handler.Handle(command, CancellationToken.None);

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

        _chunkedUploadStorage.GetSessionAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);
        _chunkedUploadStorage.GetUploadedChunkCountAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(5);
        _chunkedUploadStorage.AssembleAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(new MemoryStream([1, 2, 3]));
        _photoStorage.UploadAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("https://blob.storage/photo.jpg");
        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new CompleteChunkedUploadCommand(sessionId);

        await _handler.Handle(command, CancellationToken.None);

        Received.InOrder(() =>
        {
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>());
            _chunkedUploadStorage.CleanupSessionAsync(sessionId, Arg.Any<CancellationToken>());
        });
    }
}
