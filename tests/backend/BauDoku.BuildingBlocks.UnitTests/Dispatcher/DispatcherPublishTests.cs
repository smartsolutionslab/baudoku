using AwesomeAssertions;
using BauDoku.BuildingBlocks.Application.Events;
using BauDoku.BuildingBlocks.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BauDoku.BuildingBlocks.UnitTests.Dispatcher;

public sealed record DispatcherTestEvent(string Message) : IDomainEvent
{
    public DateTime OccurredOn => DateTime.UtcNow;
}

public sealed class DispatcherTestSuccessHandler : IDomainEventHandler<DispatcherTestEvent>
{
    public bool WasCalled { get; private set; }

    public Task Handle(DispatcherTestEvent domainEvent, CancellationToken cancellationToken = default)
    {
        WasCalled = true;
        return Task.CompletedTask;
    }
}

public sealed class DispatcherTestFailingHandler : IDomainEventHandler<DispatcherTestEvent>
{
    public Task Handle(DispatcherTestEvent domainEvent, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Handler failed");
    }
}

public sealed class DispatcherPublishTests
{
    [Fact]
    public async Task Publish_WhenHandlerThrows_ShouldContinueWithRemainingHandlers()
    {
        var failingHandler = new DispatcherTestFailingHandler();
        var successHandler = new DispatcherTestSuccessHandler();

        var services = new ServiceCollection();
        services.AddSingleton<IDomainEventHandler<DispatcherTestEvent>>(failingHandler);
        services.AddSingleton<IDomainEventHandler<DispatcherTestEvent>>(successHandler);
        services.AddSingleton(Substitute.For<ILogger<Application.Dispatcher.Dispatcher>>());
        var provider = services.BuildServiceProvider();

        var dispatcher = new Application.Dispatcher.Dispatcher(
            provider, provider.GetRequiredService<ILogger<Application.Dispatcher.Dispatcher>>());

        await dispatcher.Publish(new DispatcherTestEvent("test"));

        successHandler.WasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Publish_WhenHandlerThrows_ShouldLogError()
    {
        var failingHandler = new DispatcherTestFailingHandler();
        var logger = Substitute.For<ILogger<Application.Dispatcher.Dispatcher>>();

        var services = new ServiceCollection();
        services.AddSingleton<IDomainEventHandler<DispatcherTestEvent>>(failingHandler);
        services.AddSingleton(logger);
        var provider = services.BuildServiceProvider();

        var dispatcher = new Application.Dispatcher.Dispatcher(provider, logger);

        await dispatcher.Publish(new DispatcherTestEvent("test"));

        logger.ReceivedWithAnyArgs(1).Log(
            default, default, default(object), default, default!);
    }

    [Fact]
    public async Task Publish_WhenNoHandlerThrows_ShouldExecuteAllHandlers()
    {
        var handler1 = new DispatcherTestSuccessHandler();
        var handler2 = new DispatcherTestSuccessHandler();

        var services = new ServiceCollection();
        services.AddSingleton<IDomainEventHandler<DispatcherTestEvent>>(handler1);
        services.AddSingleton<IDomainEventHandler<DispatcherTestEvent>>(handler2);
        services.AddSingleton(Substitute.For<ILogger<Application.Dispatcher.Dispatcher>>());
        var provider = services.BuildServiceProvider();

        var dispatcher = new Application.Dispatcher.Dispatcher(
            provider, provider.GetRequiredService<ILogger<Application.Dispatcher.Dispatcher>>());

        await dispatcher.Publish(new DispatcherTestEvent("test"));

        handler1.WasCalled.Should().BeTrue();
        handler2.WasCalled.Should().BeTrue();
    }
}
