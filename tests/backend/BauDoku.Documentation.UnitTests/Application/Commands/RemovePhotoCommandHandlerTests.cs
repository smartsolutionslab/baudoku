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
    private readonly IInstallationRepository _repository;
    private readonly IPhotoStorage _photoStorage;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RemovePhotoCommandHandler _handler;

    public RemovePhotoCommandHandlerTests()
    {
        _repository = Substitute.For<IInstallationRepository>();
        _photoStorage = Substitute.For<IPhotoStorage>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new RemovePhotoCommandHandler(_repository, _photoStorage, _unitOfWork);
    }

    private static Installation CreateInstallationWithPhoto(out PhotoId photoId)
    {
        var installation = Installation.Create(
            InstallationId.New(),
            Guid.NewGuid(),
            null,
            InstallationType.CableTray,
            new GpsPosition(48.137154, 11.576124, null, 3.5, "gps"));

        photoId = PhotoId.New();
        installation.AddPhoto(photoId, "photo.jpg", "https://blob/photo.jpg", "image/jpeg", 1024,
            PhotoType.Before);

        return installation;
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldDeleteBlobAndRemovePhoto()
    {
        var installation = CreateInstallationWithPhoto(out var photoId);
        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new RemovePhotoCommand(installation.Id.Value, photoId.Value);

        await _handler.Handle(command, CancellationToken.None);

        installation.Photos.Should().BeEmpty();
        await _photoStorage.Received(1).DeleteAsync("https://blob/photo.jpg", Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInstallationNotFound_ShouldThrow()
    {
        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns((Installation?)null);

        var command = new RemovePhotoCommand(Guid.NewGuid(), Guid.NewGuid());

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_WhenPhotoNotFound_ShouldThrow()
    {
        var installation = CreateInstallationWithPhoto(out _);
        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns(installation);

        var command = new RemovePhotoCommand(installation.Id.Value, Guid.NewGuid());

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
