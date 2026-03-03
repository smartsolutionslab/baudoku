namespace BauDoku.Projects.Application.ReadModel;

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
