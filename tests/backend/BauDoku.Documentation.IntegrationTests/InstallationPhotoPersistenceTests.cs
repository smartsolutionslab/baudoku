using AwesomeAssertions;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;
using BauDoku.Documentation.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace BauDoku.Documentation.IntegrationTests;

[Collection(PostgreSqlCollection.Name)]
public sealed class InstallationPhotoPersistenceTests
{
    private readonly PostgreSqlFixture _fixture;

    public InstallationPhotoPersistenceTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddPhoto_ShouldPersistAndLoad()
    {
        var installation = Installation.Create(
            InstallationId.New(),
            Guid.NewGuid(),
            null,
            InstallationType.CableTray,
            new GpsPosition(48.1351, 11.5820, 520.0, 3.5, "internal_gps"));

        var photoId = PhotoId.New();
        installation.AddPhoto(
            photoId,
            "test-photo.jpg",
            "uploads/abc123.jpg",
            "image/jpeg",
            2048,
            PhotoType.Before,
            new Caption("Vorher-Bild"),
            new Description("Detailansicht der Kabeltrasse"),
            new GpsPosition(48.1351, 11.5820, 520.0, 3.5, "internal_gps"));

        await using (var writeContext = _fixture.CreateContext())
        {
            writeContext.Installations.Add(installation);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = _fixture.CreateContext())
        {
            var loaded = await readContext.Installations
                .Include(i => i.Photos)
                .FirstOrDefaultAsync(i => i.Id == installation.Id);

            loaded.Should().NotBeNull();
            loaded!.Photos.Should().ContainSingle();

            var photo = loaded.Photos[0];
            photo.Id.Should().Be(photoId);
            photo.FileName.Should().Be("test-photo.jpg");
            photo.BlobUrl.Should().Be("uploads/abc123.jpg");
            photo.ContentType.Should().Be("image/jpeg");
            photo.FileSize.Should().Be(2048);
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
            InstallationId.New(),
            Guid.NewGuid(),
            null,
            InstallationType.JunctionBox,
            new GpsPosition(48.0, 11.0, null, 5.0, "internal_gps"));

        installation.AddPhoto(
            PhotoId.New(),
            "minimal.png",
            "uploads/min.png",
            "image/png",
            512,
            PhotoType.Other);

        await using (var writeContext = _fixture.CreateContext())
        {
            writeContext.Installations.Add(installation);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = _fixture.CreateContext())
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
            InstallationId.New(),
            Guid.NewGuid(),
            null,
            InstallationType.Grounding,
            new GpsPosition(48.0, 11.0, null, 5.0, "internal_gps"));

        installation.AddPhoto(PhotoId.New(), "before.jpg", "url1", "image/jpeg", 1024, PhotoType.Before);
        installation.AddPhoto(PhotoId.New(), "after.jpg", "url2", "image/jpeg", 2048, PhotoType.After);
        installation.AddPhoto(PhotoId.New(), "detail.png", "url3", "image/png", 4096, PhotoType.Detail);

        await using (var writeContext = _fixture.CreateContext())
        {
            writeContext.Installations.Add(installation);
            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = _fixture.CreateContext())
        {
            var loaded = await readContext.Installations
                .Include(i => i.Photos)
                .FirstOrDefaultAsync(i => i.Id == installation.Id);

            loaded.Should().NotBeNull();
            loaded!.Photos.Should().HaveCount(3);
        }
    }
}
