using SmartSolutionsLab.BauDoku.BuildingBlocks.Domain;
using SmartSolutionsLab.BauDoku.BuildingBlocks.Guards;

namespace SmartSolutionsLab.BauDoku.Documentation.Domain;

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
