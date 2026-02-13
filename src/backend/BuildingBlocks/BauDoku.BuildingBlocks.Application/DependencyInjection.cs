using System.Reflection;
using BauDoku.BuildingBlocks.Application.Behaviors;
using BauDoku.BuildingBlocks.Application.Commands;
using BauDoku.BuildingBlocks.Application.Events;
using BauDoku.BuildingBlocks.Application.Queries;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BauDoku.BuildingBlocks.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddScoped<Dispatcher.IDispatcher, Dispatcher.Dispatcher>();

        services.AddValidatorsFromAssemblies(assemblies);

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationBehavior<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(ValidationBehaviorVoid<>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingBehavior<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(LoggingBehaviorVoid<>));

        return services;
    }
}
