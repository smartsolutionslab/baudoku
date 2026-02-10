using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Domain.Entities;

public sealed class Photo : Entity<PhotoId>
{
    public string FilePath { get; private set; } = default!;
    public Description? Description { get; private set; }
    public DateTime CapturedAt { get; private set; }
    public GpsPosition? Position { get; private set; }

    private Photo() { }

    internal static Photo Create(PhotoId id, string filePath, Description? description, GpsPosition? position)
    {
        return new Photo
        {
            Id = id,
            FilePath = filePath,
            Description = description,
            CapturedAt = DateTime.UtcNow,
            Position = position
        };
    }
}
