using BauDoku.BuildingBlocks.Domain;
using BauDoku.BuildingBlocks.Domain.Guards;

namespace BauDoku.Documentation.Domain.ValueObjects;

public sealed record UploadSessionIdentifier : IValueObject
{
    public Guid Value { get; }

    private UploadSessionIdentifier(Guid value) => Value = value;

    public static UploadSessionIdentifier From(Guid value)
    {
        Ensure.That(value).IsNotEmpty("Upload-Sitzungs-ID darf nicht leer sein.");
        return new UploadSessionIdentifier(value);
    }

    public static UploadSessionIdentifier New() => new(Guid.NewGuid());
}
