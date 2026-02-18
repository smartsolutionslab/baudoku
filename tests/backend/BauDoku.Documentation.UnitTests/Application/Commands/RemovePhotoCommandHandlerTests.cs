using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Commands.RemovePhoto;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class RemovePhotoCommandHandlerTests
{
    private readonly IInstallationRepository installations;
    private readonly IPhotoStorage photoStorage;
    private readonly IUnitOfWork unitOfWork;
    private readonly RemovePhotoCommandHandler handler;

    public RemovePhotoCommandHandlerTests()
    {
        installations = Substitute.For<IInstallationRepository>();
        photoStorage = Substitute.For<IPhotoStorage>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        handler = new RemovePhotoCommandHandler(installations, photoStorage, unitOfWork);
    }

    private static Installation CreateInstallationWithPhoto(out PhotoIdentifier photoId)
    {
        var installation = Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.CableTray,
            GpsPosition.Create(Latitude.From(48.137154), Longitude.From(11.576124), null, HorizontalAccuracy.From(3.5), GpsSource.From("gps")));

        photoId = PhotoIdentifier.New();
        installation.AddPhoto(photoId, FileName.From("photo.jpg"), BlobUrl.From("https://blob/photo.jpg"), ContentType.From("image/jpeg"), FileSize.From(1024),
            PhotoType.Before);

        return installation;
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldDeleteBlobAndRemovePhoto()
    {
        var installation = CreateInstallationWithPhoto(out var photoId);
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new RemovePhotoCommand(installation.Id.Value, photoId.Value);

        await handler.Handle(command, CancellationToken.None);

        installation.Photos.Should().BeEmpty();
        await photoStorage.Received(1).DeleteAsync(BlobUrl.From("https://blob/photo.jpg"), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInstallationNotFound_ShouldThrow()
    {
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Throws(new KeyNotFoundException());

        var command = new RemovePhotoCommand(Guid.NewGuid(), Guid.NewGuid());

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenPhotoNotFound_ShouldThrow()
    {
        var installation = CreateInstallationWithPhoto(out _);
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new RemovePhotoCommand(installation.Id.Value, Guid.NewGuid());

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
