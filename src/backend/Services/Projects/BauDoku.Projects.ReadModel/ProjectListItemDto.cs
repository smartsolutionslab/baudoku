namespace SmartSolutionsLab.BauDoku.Projects.ReadModel;

public sealed record ProjectListItemDto(
    Guid Id,
    string Name,
    string Status,
    string City,
    string ClientName,
    DateTime CreatedAt,
    int ZoneCount);
