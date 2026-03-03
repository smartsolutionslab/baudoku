using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.ReadModel;

public sealed record InstallationListFilter(
    ProjectIdentifier? ProjectId = null,
    ZoneIdentifier? ZoneId = null,
    InstallationType? Type = null,
    InstallationStatus? Status = null,
    SearchTerm? Search = null);
