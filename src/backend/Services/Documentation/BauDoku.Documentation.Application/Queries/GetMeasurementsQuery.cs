using BauDoku.BuildingBlocks.Application.Queries;
using BauDoku.Documentation.ReadModel;
using BauDoku.Documentation.Domain;

namespace BauDoku.Documentation.Application.Queries;

public sealed record GetMeasurementsQuery(InstallationIdentifier InstallationId) : IQuery<IReadOnlyList<MeasurementDto>>;
