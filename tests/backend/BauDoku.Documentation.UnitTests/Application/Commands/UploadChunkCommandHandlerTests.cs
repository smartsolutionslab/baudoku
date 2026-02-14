using AwesomeAssertions;
using BauDoku.Documentation.Application.Commands.UploadChunk;
using BauDoku.Documentation.Application.Contracts;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class UploadChunkCommandHandlerTests
{
    private readonly IChunkedUploadStorage chunkedUploadStorage;
    private readonly UploadChunkCommandHandler handler;

    public UploadChunkCommandHandlerTests()
    {
        chunkedUploadStorage = Substitute.For<IChunkedUploadStorage>();
        handler = new UploadChunkCommandHandler(chunkedUploadStorage);
    }

    private static ChunkedUploadSession CreateValidSession(Guid sessionId) =>
        new(sessionId, Guid.NewGuid(), "photo.jpg", "image/jpeg",
            5 * 1024 * 1024, 5, "before", null, null, null, null, null, null, null,
            DateTime.UtcNow);

    [Fact]
    public async Task Handle_WithValidCommand_ShouldStoreChunk()
    {
        var sessionId = Guid.NewGuid();
        var session = CreateValidSession(sessionId);
        chunkedUploadStorage.GetSessionAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);

        var stream = new MemoryStream([1, 2, 3]);
        var command = new UploadChunkCommand(sessionId, 2, stream);

        await handler.Handle(command, CancellationToken.None);

        await chunkedUploadStorage.Received(1)
            .StoreChunkAsync(sessionId, 2, stream, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSessionNotFound_ShouldThrow()
    {
        chunkedUploadStorage.GetSessionAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((ChunkedUploadSession?)null);

        var command = new UploadChunkCommand(Guid.NewGuid(), 0, new MemoryStream([1, 2, 3]));

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_WhenChunkIndexOutOfRange_ShouldThrow()
    {
        var sessionId = Guid.NewGuid();
        var session = CreateValidSession(sessionId);
        chunkedUploadStorage.GetSessionAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);

        var command = new UploadChunkCommand(sessionId, 5, new MemoryStream([1, 2, 3]));

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
