using AwesomeAssertions;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.Events;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.UnitTests.Domain.Aggregates;

public sealed class InstallationPhotoTests
{
    private static Installation CreateValidInstallation()
    {
        return Installation.Create(
            InstallationId.New(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            InstallationType.CableTray,
            new GpsPosition(48.1351, 11.5820, 520.0, 3.5, "internal_gps"),
            new Description("Kabeltrasse im Erdgeschoss"));
    }

    [Fact]
    public void AddPhoto_ShouldAddPhotoToCollection()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();
        var photoId = PhotoId.New();

        installation.AddPhoto(
            photoId,
            "photo.jpg",
            "uploads/abc.jpg",
            "image/jpeg",
            1024,
            PhotoType.Before,
            new Caption("Vorher-Bild"),
            new Description("Detailansicht"),
            new GpsPosition(48.1351, 11.5820, 520.0, 3.5, "internal_gps"));

        installation.Photos.Should().ContainSingle();
        var photo = installation.Photos[0];
        photo.Id.Should().Be(photoId);
        photo.FileName.Should().Be("photo.jpg");
        photo.BlobUrl.Should().Be("uploads/abc.jpg");
        photo.ContentType.Should().Be("image/jpeg");
        photo.FileSize.Should().Be(1024);
        photo.PhotoType.Should().Be(PhotoType.Before);
        photo.Caption!.Value.Should().Be("Vorher-Bild");
        photo.Description!.Value.Should().Be("Detailansicht");
        photo.Position.Should().NotBeNull();
    }

    [Fact]
    public void AddPhoto_ShouldRaisePhotoAddedEvent()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();
        var photoId = PhotoId.New();

        installation.AddPhoto(photoId, "photo.jpg", "url", "image/jpeg", 1024, PhotoType.Detail);

        installation.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<PhotoAdded>()
            .Which.PhotoId.Should().Be(photoId);
    }

    [Fact]
    public void AddPhoto_WithOptionalFieldsNull_ShouldSucceed()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();

        installation.AddPhoto(
            PhotoId.New(), "photo.jpg", "url", "image/jpeg", 2048, PhotoType.Other);

        var photo = installation.Photos[0];
        photo.Caption.Should().BeNull();
        photo.Description.Should().BeNull();
        photo.Position.Should().BeNull();
    }

    [Fact]
    public void AddPhoto_WhenCompleted_ShouldThrowBusinessRuleException()
    {
        var installation = CreateValidInstallation();
        installation.MarkAsCompleted();

        var act = () => installation.AddPhoto(
            PhotoId.New(), "photo.jpg", "url", "image/jpeg", 1024, PhotoType.After);

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void RemovePhoto_ShouldRemovePhotoFromCollection()
    {
        var installation = CreateValidInstallation();
        var photoId = PhotoId.New();
        installation.AddPhoto(photoId, "photo.jpg", "url", "image/jpeg", 1024, PhotoType.Before);
        installation.ClearDomainEvents();

        installation.RemovePhoto(photoId);

        installation.Photos.Should().BeEmpty();
    }

    [Fact]
    public void RemovePhoto_ShouldRaisePhotoRemovedEvent()
    {
        var installation = CreateValidInstallation();
        var photoId = PhotoId.New();
        installation.AddPhoto(photoId, "photo.jpg", "url", "image/jpeg", 1024, PhotoType.Before);
        installation.ClearDomainEvents();

        installation.RemovePhoto(photoId);

        installation.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<PhotoRemoved>()
            .Which.PhotoId.Should().Be(photoId);
    }

    [Fact]
    public void RemovePhoto_WithNonExistentId_ShouldThrowInvalidOperation()
    {
        var installation = CreateValidInstallation();

        var act = () => installation.RemovePhoto(PhotoId.New());

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void RemovePhoto_WhenCompleted_ShouldThrowBusinessRuleException()
    {
        var installation = CreateValidInstallation();
        var photoId = PhotoId.New();
        installation.AddPhoto(photoId, "photo.jpg", "url", "image/jpeg", 1024, PhotoType.Before);
        installation.MarkAsCompleted();

        var act = () => installation.RemovePhoto(photoId);

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void AddMultiplePhotos_ShouldTrackAll()
    {
        var installation = CreateValidInstallation();

        installation.AddPhoto(PhotoId.New(), "photo1.jpg", "url1", "image/jpeg", 1024, PhotoType.Before);
        installation.AddPhoto(PhotoId.New(), "photo2.jpg", "url2", "image/jpeg", 2048, PhotoType.After);
        installation.AddPhoto(PhotoId.New(), "photo3.png", "url3", "image/png", 4096, PhotoType.Detail);

        installation.Photos.Should().HaveCount(3);
    }
}
