using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Entities;

public sealed class Photo : Entity<PhotoIdentifier>
{
    public string FileName { get; private set; } = default!;
    public string BlobUrl { get; private set; } = default!;
    public string ContentType { get; private set; } = default!;
    public long FileSize { get; private set; }
    public PhotoType PhotoType { get; private set; } = default!;
    public Caption? Caption { get; private set; }
    public Description? Description { get; private set; }
    public DateTime TakenAt { get; private set; }
    public GpsPosition? Position { get; private set; }

    private Photo() { }

    internal static Photo Create(
        PhotoIdentifier id,
        string fileName,
        string blobUrl,
        string contentType,
        long fileSize,
        PhotoType photoType,
        Caption? caption,
        Description? description,
        GpsPosition? position,
        DateTime? takenAt = null)
    {
        return new Photo
        {
            Id = id,
            FileName = fileName,
            BlobUrl = blobUrl,
            ContentType = contentType,
            FileSize = fileSize,
            PhotoType = photoType,
            Caption = caption,
            Description = description,
            TakenAt = takenAt ?? DateTime.UtcNow,
            Position = position
        };
    }
}
