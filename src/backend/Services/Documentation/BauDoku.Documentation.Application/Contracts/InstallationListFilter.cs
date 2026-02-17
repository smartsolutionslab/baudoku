namespace BauDoku.Documentation.Application.Contracts;

public sealed record InstallationListFilter(
    Guid? ProjectId = null,
    Guid? ZoneId = null,
    string? Type = null,
    string? Status = null,
    string? Search = null);
