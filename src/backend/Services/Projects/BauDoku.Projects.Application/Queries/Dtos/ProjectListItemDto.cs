namespace BauDoku.Projects.Application.Queries.Dtos;

public sealed record ProjectListItemDto(
    Guid Id,
    string Name,
    string Status,
    string City,
    string ClientName,
    DateTime CreatedAt,
    int ZoneCount);
