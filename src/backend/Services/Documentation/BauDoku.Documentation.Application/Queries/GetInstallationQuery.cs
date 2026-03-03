using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.Application.ReadModel;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Queries;

public sealed record GetInstallationQuery(InstallationIdentifier InstallationId) : IQuery<InstallationDto>;
