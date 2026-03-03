using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.BuildingBlocks.Persistence;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.ReadModel;
using BauDoku.Projects.Domain;
using BauDoku.Projects.Infrastructure.Persistence;
using BauDoku.Projects.Infrastructure.Persistence.Repositories;
using BauDoku.Projects.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BauDoku.Projects.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddProjectsInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ProjectsDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddDbContext<ProjectsReadDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork<ProjectsDbContext>>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IProjectReadRepository, ProjectReadRepository>();
        services.AddScoped<IProjectCountProvider, ProjectCountProvider>();
        services.AddHostedService<ActiveProjectCountService>();

        return services;
    }
}
