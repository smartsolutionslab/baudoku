using BauDoku.ResourceService.Services;
using k8s;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001, listenOptions =>
        listenOptions.Protocols = HttpProtocols.Http2);
});

builder.Services.AddGrpc();

builder.Services.AddSingleton<IKubernetes>(_ =>
{
    var config = KubernetesClientConfiguration.IsInCluster()
        ? KubernetesClientConfiguration.InClusterConfig()
        : KubernetesClientConfiguration.BuildConfigFromConfigFile();
    return new Kubernetes(config);
});

builder.Services.AddSingleton<PodWatcher>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<PodWatcher>());
builder.Services.AddSingleton<PodLogStreamer>();

var app = builder.Build();

app.MapGrpcService<KubernetesDashboardService>();

app.Run();
