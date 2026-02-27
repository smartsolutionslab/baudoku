using BauDoku.BuildingBlocks.Domain;

namespace BauDoku.Documentation.Domain;

public sealed class Photo : Entity<PhotoIdentifier>
{
    public FileName FileName { get; private set; } = default!;
    public BlobUrl BlobUrl { get; private set; } = default!;
    public ContentType ContentType { get; private set; } = default!;
    public FileSize FileSize { get; private set; } = default!;
    public PhotoType PhotoType { get; private set; } = default!;
    public Caption? Caption { get; private set; }
    public Description? Description { get; private set; }
    public DateTime TakenAt { get; private set; }
    public GpsPosition? Position { get; private set; }

    private Photo() { }

    internal static Photo Create(
        PhotoIdentifier id,
        FileName fileName,
        BlobUrl blobUrl,
        ContentType contentType,
        FileSize fileSize,
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

    internal static Photo Reconstitute(
        PhotoIdentifier id,
        FileName fileName,
        BlobUrl blobUrl,
        ContentType contentType,
        FileSize fileSize,
        PhotoType photoType,
        Caption? caption,
        Description? description,
        GpsPosition? position,
        DateTime takenAt)
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
            TakenAt = takenAt,
            Position = position
        };
    }
}
