using BauDoku.BuildingBlocks.Domain;
using BauDoku.Documentation.Domain.Aggregates;
using BauDoku.Documentation.Domain.ValueObjects;

namespace BauDoku.Documentation.Application.Contracts;

public interface IInstallationRepository : IRepository<Installation, InstallationId>;
