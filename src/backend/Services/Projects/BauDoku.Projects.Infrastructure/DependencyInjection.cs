using BauDoku.BuildingBlocks.Application.Persistence;
using BauDoku.Projects.Application.Contracts;
using BauDoku.Projects.Infrastructure.Persistence;
using BauDoku.Projects.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BauDoku.Projects.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddProjectsInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ProjectsDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ProjectsDbContext>());
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IProjectReadRepository, ProjectReadRepository>();

        return services;
    }
}
