using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Contracts;

public sealed record InstallationListFilter(
    ProjectIdentifier? ProjectId = null,
    ZoneIdentifier? ZoneId = null,
    InstallationType? Type = null,
    InstallationStatus? Status = null,
    string? Search = null);
