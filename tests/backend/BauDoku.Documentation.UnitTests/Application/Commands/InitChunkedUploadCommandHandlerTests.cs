using AwesomeAssertions;
using BauDoku.Documentation.Application.Commands.InitChunkedUpload;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class InitChunkedUploadCommandHandlerTests
{
    private readonly IInstallationRepository _repository;
    private readonly IChunkedUploadStorage _chunkedUploadStorage;
    private readonly InitChunkedUploadCommandHandler _handler;

    public InitChunkedUploadCommandHandlerTests()
    {
        _repository = Substitute.For<IInstallationRepository>();
        _chunkedUploadStorage = Substitute.For<IChunkedUploadStorage>();
        _handler = new InitChunkedUploadCommandHandler(_repository, _chunkedUploadStorage);
    }

    private static Installation CreateValidInstallation() =>
        Installation.Create(
            InstallationId.New(),
            Guid.NewGuid(),
            null,
            InstallationType.CableTray,
            new GpsPosition(48.137154, 11.576124, null, 3.5, "gps"));

    private static InitChunkedUploadCommand CreateValidCommand(Guid installationId) =>
        new(installationId, "photo.jpg", "image/jpeg", 5 * 1024 * 1024, 5,
            "before", null, null, null, null, null, null, null);

    [Fact]
    public async Task Handle_WithValidCommand_ShouldInitSession()
    {
        var installation = CreateValidInstallation();
        var expectedSessionId = Guid.NewGuid();
        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns(installation);
        _chunkedUploadStorage.InitSessionAsync(Arg.Any<ChunkedUploadSession>(), Arg.Any<CancellationToken>())
            .Returns(expectedSessionId);

        var command = CreateValidCommand(installation.Id.Value);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
        await _chunkedUploadStorage.Received(1)
            .InitSessionAsync(Arg.Is<ChunkedUploadSession>(s =>
                s.InstallationId == installation.Id.Value &&
                s.FileName == "photo.jpg" &&
                s.TotalChunks == 5), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInstallationNotFound_ShouldThrow()
    {
        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns((Installation?)null);

        var command = CreateValidCommand(Guid.NewGuid());

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
