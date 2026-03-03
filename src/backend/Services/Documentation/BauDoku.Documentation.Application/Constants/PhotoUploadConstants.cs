namespace BauDoku.Documentation.Application.Constants;

public static class PhotoUploadConstants
{
    public const long MaxFileSize = 50 * 1024 * 1024; // 50 MB
    public const int MaxChunks = 50;

    public static readonly HashSet<string> AllowedContentTypes = ["image/jpeg", "image/png", "image/heic"];
}
