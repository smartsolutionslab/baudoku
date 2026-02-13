using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Commands.RemovePhoto;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class RemovePhotoCommandHandlerTests
{
    private readonly IInstallationRepository repository;
    private readonly IPhotoStorage photoStorage;
    private readonly IUnitOfWork unitOfWork;
    private readonly RemovePhotoCommandHandler handler;

    public RemovePhotoCommandHandlerTests()
    {
        repository = Substitute.For<IInstallationRepository>();
        photoStorage = Substitute.For<IPhotoStorage>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        handler = new RemovePhotoCommandHandler(repository, photoStorage, unitOfWork);
    }

    private static Installation CreateInstallationWithPhoto(out PhotoIdentifier photoId)
    {
        var installation = Installation.Create(
            InstallationIdentifier.New(),
            Guid.NewGuid(),
            null,
            InstallationType.CableTray,
            GpsPosition.Create(48.137154, 11.576124, null, 3.5, "gps"));

        photoId = PhotoIdentifier.New();
        installation.AddPhoto(photoId, "photo.jpg", "https://blob/photo.jpg", "image/jpeg", 1024,
            PhotoType.Before);

        return installation;
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldDeleteBlobAndRemovePhoto()
    {
        var installation = CreateInstallationWithPhoto(out var photoId);
        repository.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new RemovePhotoCommand(installation.Id.Value, photoId.Value);

        await handler.Handle(command, CancellationToken.None);

        installation.Photos.Should().BeEmpty();
        await photoStorage.Received(1).DeleteAsync("https://blob/photo.jpg", Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInstallationNotFound_ShouldThrow()
    {
        repository.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns((Installation?)null);

        var command = new RemovePhotoCommand(Guid.NewGuid(), Guid.NewGuid());

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_WhenPhotoNotFound_ShouldThrow()
    {
        var installation = CreateInstallationWithPhoto(out _);
        repository.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new RemovePhotoCommand(installation.Id.Value, Guid.NewGuid());

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
