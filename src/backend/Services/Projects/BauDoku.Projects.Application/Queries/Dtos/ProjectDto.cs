namespace BauDoku.Projects.Application.Queries.Dtos;

public sealed record ProjectDto(
    Guid Id,
    string Name,
    string Status,
    string Street,
    string City,
    string ZipCode,
    string ClientName,
    string? ClientEmail,
    string? ClientPhone,
    DateTime CreatedAt,
    IReadOnlyList<ZoneDto> Zones);
