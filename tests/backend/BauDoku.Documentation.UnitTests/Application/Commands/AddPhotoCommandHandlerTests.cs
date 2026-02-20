using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Documentation.Application.Commands;
using BauDoku.Documentation.Application.Commands.Handlers;
using BauDoku.Documentation.Application.Contracts;
using BauDoku.Documentation.Domain;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BauDoku.Documentation.UnitTests.Application.Commands;

public sealed class AddPhotoCommandHandlerTests
{
    private readonly IInstallationRepository installations;
    private readonly IPhotoStorage photoStorage;
    private readonly IUnitOfWork unitOfWork;
    private readonly AddPhotoCommandHandler handler;

    public AddPhotoCommandHandlerTests()
    {
        installations = Substitute.For<IInstallationRepository>();
        photoStorage = Substitute.For<IPhotoStorage>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        handler = new AddPhotoCommandHandler(installations, photoStorage, unitOfWork);
    }

    private static Installation CreateValidInstallation() =>
        Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.CableTray,
            GpsPosition.Create(Latitude.From(48.137154), Longitude.From(11.576124), null, HorizontalAccuracy.From(3.5), GpsSource.From("gps")));

    private static AddPhotoCommand CreateValidCommand(InstallationIdentifier installationId) =>
        new(installationId, FileName.From("photo.jpg"), ContentType.From("image/jpeg"), FileSize.From(1024 * 100), PhotoType.Before,
            null, null, null, null, null, null, null, new MemoryStream([1, 2, 3]));

    [Fact]
    public async Task Handle_WithValidCommand_ShouldUploadAndAddPhoto()
    {
        var installation = CreateValidInstallation();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);
        photoStorage.UploadAsync(Arg.Any<Stream>(), Arg.Any<FileName>(), Arg.Any<ContentType>(), Arg.Any<CancellationToken>())
            .Returns(BlobUrl.From("https://blob.storage/photo.jpg"));

        var command = CreateValidCommand(installation.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
        installation.Photos.Should().ContainSingle();
        await photoStorage.Received(1).UploadAsync(Arg.Any<Stream>(), FileName.From("photo.jpg"), ContentType.From("image/jpeg"), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenInstallationNotFound_ShouldThrow()
    {
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Throws(new KeyNotFoundException());

        var command = CreateValidCommand(InstallationIdentifier.New());

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WithGpsPosition_ShouldSetPhotoPosition()
    {
        var installation = CreateValidInstallation();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);
        photoStorage.UploadAsync(Arg.Any<Stream>(), Arg.Any<FileName>(), Arg.Any<ContentType>(), Arg.Any<CancellationToken>())
            .Returns(BlobUrl.From("https://blob.storage/photo.jpg"));

        var command = new AddPhotoCommand(
            installation.Id, FileName.From("photo.jpg"), ContentType.From("image/jpeg"), FileSize.From(1024), PhotoType.Before,
            null, null, Latitude.From(48.0), Longitude.From(11.0), 500.0, HorizontalAccuracy.From(5.0), GpsSource.From("gps"),
            new MemoryStream([1, 2, 3]));

        await handler.Handle(command, CancellationToken.None);

        installation.Photos.Should().ContainSingle();
        installation.Photos[0].Position.Should().NotBeNull();
        installation.Photos[0].Position!.Latitude.Value.Should().Be(48.0);
    }

    [Fact]
    public async Task Handle_WithoutGpsPosition_ShouldNotSetPhotoPosition()
    {
        var installation = CreateValidInstallation();
        installations.GetByIdAsync(Arg.Any<InstallationIdentifier>(), Arg.Any<CancellationToken>())
            .Returns(installation);
        photoStorage.UploadAsync(Arg.Any<Stream>(), Arg.Any<FileName>(), Arg.Any<ContentType>(), Arg.Any<CancellationToken>())
            .Returns(BlobUrl.From("https://blob.storage/photo.jpg"));

        var command = CreateValidCommand(installation.Id);

        await handler.Handle(command, CancellationToken.None);

        installation.Photos.Should().ContainSingle();
        installation.Photos[0].Position.Should().BeNull();
    }
}
