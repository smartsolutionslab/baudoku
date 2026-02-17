using AwesomeAssertions;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using BauDoku.Documentation.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Documentation.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class InstallationPhotoPersistenceTests(PostgreSqlFixture fixture)
{
    [Fact]
    public async Task AddPhoto_ShouldPersistAndLoad()
    {
        var installation = Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.CableTray,
            GpsPosition.Create(48.1351, 11.5820, 520.0, 3.5, "internal_gps"));

        var photoId = PhotoIdentifier.New();
        installation.AddPhoto(
            photoId,
            FileName.From("test-photo.jpg"),
            BlobUrl.From("uploads/abc123.jpg"),
            ContentType.From("image/jpeg"),
            FileSize.From(2048),
            PhotoType.Before,
            Caption.From("Vorher-Bild"),
            Description.From("Detailansicht der Kabeltrasse"),
            GpsPosition.Create(48.1351, 11.5820, 520.0, 3.5, "internal_gps"));

        await using (var writeContext = fixture.CreateContext())
        {
            writeContext.Installations.Add(installation);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = fixture.CreateContext())
        {
            var loaded = await readContext.Installations
                .Include(i => i.Photos)
                .FirstOrDefaultAsync(i => i.Id == installation.Id);

            loaded.Should().NotBeNull();
            loaded!.Photos.Should().ContainSingle();

            var photo = loaded.Photos[0];
            photo.Id.Should().Be(photoId);
            photo.FileName.Value.Should().Be("test-photo.jpg");
            photo.BlobUrl.Value.Should().Be("uploads/abc123.jpg");
            photo.ContentType.Value.Should().Be("image/jpeg");
            photo.FileSize.Value.Should().Be(2048);
            photo.PhotoType.Should().Be(PhotoType.Before);
            photo.Caption!.Value.Should().Be("Vorher-Bild");
            photo.Description!.Value.Should().Be("Detailansicht der Kabeltrasse");
            photo.Position.Should().NotBeNull();
            photo.Position!.Latitude.Should().Be(48.1351);
            photo.TakenAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }
    }

    [Fact]
    public async Task AddPhoto_WithOptionalFieldsNull_ShouldPersist()
    {
        var installation = Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.JunctionBox,
            GpsPosition.Create(48.0, 11.0, null, 5.0, "internal_gps"));

        installation.AddPhoto(
            PhotoIdentifier.New(),
            FileName.From("minimal.png"),
            BlobUrl.From("uploads/min.png"),
            ContentType.From("image/png"),
            FileSize.From(512),
            PhotoType.Other);

        await using (var writeContext = fixture.CreateContext())
        {
            writeContext.Installations.Add(installation);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = fixture.CreateContext())
        {
            var loaded = await readContext.Installations
                .Include(i => i.Photos)
                .FirstOrDefaultAsync(i => i.Id == installation.Id);

            loaded.Should().NotBeNull();
            var photo = loaded!.Photos[0];
            photo.Caption.Should().BeNull();
            photo.Description.Should().BeNull();
            photo.Position.Should().BeNull();
            photo.PhotoType.Should().Be(PhotoType.Other);
        }
    }

    [Fact]
    public async Task AddMultiplePhotos_ShouldPersistAll()
    {
        var installation = Installation.Create(
            InstallationIdentifier.New(),
            ProjectIdentifier.New(),
            null,
            InstallationType.Grounding,
            GpsPosition.Create(48.0, 11.0, null, 5.0, "internal_gps"));

        installation.AddPhoto(PhotoIdentifier.New(), FileName.From("before.jpg"), BlobUrl.From("url1"), ContentType.From("image/jpeg"), FileSize.From(1024), PhotoType.Before);
        installation.AddPhoto(PhotoIdentifier.New(), FileName.From("after.jpg"), BlobUrl.From("url2"), ContentType.From("image/jpeg"), FileSize.From(2048), PhotoType.After);
        installation.AddPhoto(PhotoIdentifier.New(), FileName.From("detail.png"), BlobUrl.From("url3"), ContentType.From("image/png"), FileSize.From(4096), PhotoType.Detail);

        await using (var writeContext = fixture.CreateContext())
        {
            writeContext.Installations.Add(installation);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = fixture.CreateContext())
        {
            var loaded = await readContext.Installations
                .Include(i => i.Photos)
                .FirstOrDefaultAsync(i => i.Id == installation.Id);

            loaded.Should().NotBeNull();
            loaded!.Photos.Should().HaveCount(3);
        }
    }
}
