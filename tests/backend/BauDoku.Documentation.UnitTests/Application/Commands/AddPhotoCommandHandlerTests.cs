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
    private readonly IInstallationRepository repository;
    private readonly IPhotoStorage photoStorage;
    private readonly IUnitOfWork unitOfWork;
    private readonly AddPhotoCommandHandler handler;

    public AddPhotoCommandHandlerTests()
    {
        repository = Substitute.For<IInstallationRepository>();
        photoStorage = Substitute.For<IPhotoStorage>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        handler = new AddPhotoCommandHandler(repository, photoStorage, unitOfWork);
    }

    private static Installation CreateValidInstallation() =>
        Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.CableTray,
            GpsPosition.Create(48.137154, 11.576124, null, 3.5, "gps"));

    private static AddPhotoCommand CreateValidCommand(Guid installationId) =>
        new(installationId, "photo.jpg", "image/jpeg", 1024 * 100, "before",
            null, null, null, null, null, null, null, new MemoryStream([1, 2, 3]));

    [Fact]
    public async Task Handle_WithValidCommand_ShouldUploadAndAddPhoto()
    {
        var installation = CreateValidInstallation();
        repository.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);
        photoStorage.UploadAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("https://blob.storage/photo.jpg");

        var command = CreateValidCommand(installation.Id.Value);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
        installation.Photos.Should().ContainSingle();
        await photoStorage.Received(1).UploadAsync(Arg.Any<Stream>(), "photo.jpg", "image/jpeg", Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInstallationNotFound_ShouldThrow()
    {
        repository.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns((Installation?)null);

        var command = CreateValidCommand(Guid.NewGuid());

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_WithGpsPosition_ShouldSetPhotoPosition()
    {
        var installation = CreateValidInstallation();
        repository.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);
        photoStorage.UploadAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("https://blob.storage/photo.jpg");

        var command = new AddPhotoCommand(
            installation.Id.Value, "photo.jpg", "image/jpeg", 1024, "before",
            null, null, 48.0, 11.0, 500.0, 5.0, "gps", new MemoryStream([1, 2, 3]));

        await handler.Handle(command, CancellationToken.None);

        installation.Photos.Should().ContainSingle();
        installation.Photos[0].Position.Should().NotBeNull();
        installation.Photos[0].Position!.Latitude.Should().Be(48.0);
    }

    [Fact]
    public async Task Handle_WithoutGpsPosition_ShouldNotSetPhotoPosition()
    {
        var installation = CreateValidInstallation();
        repository.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);
        photoStorage.UploadAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("https://blob.storage/photo.jpg");

        var command = CreateValidCommand(installation.Id.Value);

        await handler.Handle(command, CancellationToken.None);

        installation.Photos.Should().ContainSingle();
        installation.Photos[0].Position.Should().BeNull();
    }
}
