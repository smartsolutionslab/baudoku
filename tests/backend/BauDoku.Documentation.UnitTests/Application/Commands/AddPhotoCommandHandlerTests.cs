using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Commands.AddPhoto;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using NSubstitute;

namespace BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class AddPhotoCommandHandlerTests
{
    private readonly IInstallationRepository _repository;
    private readonly IPhotoStorage _photoStorage;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AddPhotoCommandHandler _handler;

    public AddPhotoCommandHandlerTests()
    {
        _repository = Substitute.For<IInstallationRepository>();
        _photoStorage = Substitute.For<IPhotoStorage>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new AddPhotoCommandHandler(_repository, _photoStorage, _unitOfWork);
    }

    private static Installation CreateValidInstallation() =>
        Installation.Create(
            InstallationId.New(),
            Guid.NewGuid(),
            null,
            InstallationType.CableTray,
            new GpsPosition(48.137154, 11.576124, null, 3.5, "gps"));

    private static AddPhotoCommand CreateValidCommand(Guid installationId) =>
        new(installationId, "photo.jpg", "image/jpeg", 1024 * 100, "before",
            null, null, null, null, null, null, null, new MemoryStream([1, 2, 3]));

    [Fact]
    public async Task Handle_WithValidCommand_ShouldUploadAndAddPhoto()
    {
        var installation = CreateValidInstallation();
        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns(installation);
        _photoStorage.UploadAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("https://blob.storage/photo.jpg");

        var command = CreateValidCommand(installation.Id.Value);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
        installation.Photos.Should().ContainSingle();
        await _photoStorage.Received(1).UploadAsync(Arg.Any<Stream>(), "photo.jpg", "image/jpeg", Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
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

    [Fact]
    public async Task Handle_WithGpsPosition_ShouldSetPhotoPosition()
    {
        var installation = CreateValidInstallation();
        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns(installation);
        _photoStorage.UploadAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("https://blob.storage/photo.jpg");

        var command = new AddPhotoCommand(
            installation.Id.Value, "photo.jpg", "image/jpeg", 1024, "before",
            null, null, 48.0, 11.0, 500.0, 5.0, "gps", new MemoryStream([1, 2, 3]));

        await _handler.Handle(command, CancellationToken.None);

        installation.Photos.Should().ContainSingle();
        installation.Photos[0].Position.Should().NotBeNull();
        installation.Photos[0].Position!.Latitude.Should().Be(48.0);
    }

    [Fact]
    public async Task Handle_WithoutGpsPosition_ShouldNotSetPhotoPosition()
    {
        var installation = CreateValidInstallation();
        _repository.GetByIdAsync(Arg.Any<InstallationId>(), Arg.Any<CancellationToken>())
            .Returns(installation);
        _photoStorage.UploadAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("https://blob.storage/photo.jpg");

        var command = CreateValidCommand(installation.Id.Value);

        await _handler.Handle(command, CancellationToken.None);

        installation.Photos.Should().ContainSingle();
        installation.Photos[0].Position.Should().BeNull();
    }
}
