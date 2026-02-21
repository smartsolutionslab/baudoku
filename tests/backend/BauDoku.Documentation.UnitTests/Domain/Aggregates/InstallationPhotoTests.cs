using AwesomeAssertions;
using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain;
using BauDoku.Documentation.UnitTests.Builders;

namespace BauDoku.Documentation.UnitTests.Domain.Aggregates;

public sealed class InstallationPhotoTests
{
    private static Installation CreateValidInstallation() => new InstallationBuilder().Build();

    [Fact]
    public void AddPhoto_ShouldAddPhotoToCollection()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();
        var photoId = PhotoIdentifier.New();

        installation.AddPhoto(
            photoId,
            FileName.From("photo.jpg"),
            BlobUrl.From("uploads/abc.jpg"),
            ContentType.From("image/jpeg"),
            FileSize.From(1024),
            PhotoType.Before,
            Caption.From("Vorher-Bild"),
            Description.From("Detailansicht"),
            GpsPosition.Create(Latitude.From(48.1351), Longitude.From(11.5820), 520.0, HorizontalAccuracy.From(3.5), GpsSource.From("internal_gps")));

        installation.Photos.Should().ContainSingle();
        var photo = installation.Photos[0];
        photo.Id.Should().Be(photoId);
        photo.FileName.Value.Should().Be("photo.jpg");
        photo.BlobUrl.Value.Should().Be("uploads/abc.jpg");
        photo.ContentType.Value.Should().Be("image/jpeg");
        photo.FileSize.Value.Should().Be(1024);
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
        var photoId = PhotoIdentifier.New();

        installation.AddPhoto(photoId, FileName.From("photo.jpg"), BlobUrl.From("url"), ContentType.From("image/jpeg"), FileSize.From(1024), PhotoType.Detail);

        installation.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<PhotoAdded>()
            .Which.PhotoIdentifier.Should().Be(photoId);
    }

    [Fact]
    public void AddPhoto_WithOptionalFieldsNull_ShouldSucceed()
    {
        var installation = CreateValidInstallation();
        installation.ClearDomainEvents();

        installation.AddPhoto(
            PhotoIdentifier.New(), FileName.From("photo.jpg"), BlobUrl.From("url"), ContentType.From("image/jpeg"), FileSize.From(2048), PhotoType.Other);

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
            PhotoIdentifier.New(), FileName.From("photo.jpg"), BlobUrl.From("url"), ContentType.From("image/jpeg"), FileSize.From(1024), PhotoType.After);

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void RemovePhoto_ShouldRemovePhotoFromCollection()
    {
        var installation = CreateValidInstallation();
        var photoId = PhotoIdentifier.New();
        installation.AddPhoto(photoId, FileName.From("photo.jpg"), BlobUrl.From("url"), ContentType.From("image/jpeg"), FileSize.From(1024), PhotoType.Before);
        installation.ClearDomainEvents();

        installation.RemovePhoto(photoId);

        installation.Photos.Should().BeEmpty();
    }

    [Fact]
    public void RemovePhoto_ShouldRaisePhotoRemovedEvent()
    {
        var installation = CreateValidInstallation();
        var photoId = PhotoIdentifier.New();
        installation.AddPhoto(photoId, FileName.From("photo.jpg"), BlobUrl.From("url"), ContentType.From("image/jpeg"), FileSize.From(1024), PhotoType.Before);
        installation.ClearDomainEvents();

        installation.RemovePhoto(photoId);

        installation.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<PhotoRemoved>()
            .Which.PhotoIdentifier.Should().Be(photoId);
    }

    [Fact]
    public void RemovePhoto_WithNonExistentId_ShouldThrowInvalidOperation()
    {
        var installation = CreateValidInstallation();

        var act = () => installation.RemovePhoto(PhotoIdentifier.New());

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void RemovePhoto_WhenCompleted_ShouldThrowBusinessRuleException()
    {
        var installation = CreateValidInstallation();
        var photoId = PhotoIdentifier.New();
        installation.AddPhoto(photoId, FileName.From("photo.jpg"), BlobUrl.From("url"), ContentType.From("image/jpeg"), FileSize.From(1024), PhotoType.Before);
        installation.MarkAsCompleted();

        var act = () => installation.RemovePhoto(photoId);

        act.Should().Throw<BusinessRuleException>();
    }

    [Fact]
    public void AddMultiplePhotos_ShouldTrackAll()
    {
        var installation = CreateValidInstallation();

        installation.AddPhoto(PhotoIdentifier.New(), FileName.From("photo1.jpg"), BlobUrl.From("url1"), ContentType.From("image/jpeg"), FileSize.From(1024), PhotoType.Before);
        installation.AddPhoto(PhotoIdentifier.New(), FileName.From("photo2.jpg"), BlobUrl.From("url2"), ContentType.From("image/jpeg"), FileSize.From(2048), PhotoType.After);
        installation.AddPhoto(PhotoIdentifier.New(), FileName.From("photo3.png"), BlobUrl.From("url3"), ContentType.From("image/png"), FileSize.From(4096), PhotoType.Detail);

        installation.Photos.Should().HaveCount(3);
    }
}
