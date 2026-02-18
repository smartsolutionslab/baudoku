using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;

namespace BauDoku.ServiceDefaults;

public static class Extensions
{
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder, Action<IHealthChecksBuilder>? configureHealthChecks = null)
    {
        builder.ConfigureSerilog();

        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks(configureHealthChecks);

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
            http.AddServiceDiscovery();
        });

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        app.UseExceptionHandler();

        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = static (diagnosticContext, httpContext) =>
            {
                var remoteIpAddress = httpContext.Connection.RemoteIpAddress;
                var userAgent = httpContext.Request.Headers.UserAgent;
                diagnosticContext.Set("ClientIP", remoteIpAddress?.ToString() ?? "-");
                diagnosticContext.Set("UserAgent", userAgent.ToString());
            };
            options.GetLevel = static (httpContext, _, _) =>
            {
                var request = httpContext.Request;

                return request.Path.StartsWithSegments("/health") || request.Path.StartsWithSegments("/alive")
                    ? LogEventLevel.Verbose
                    : LogEventLevel.Information;
            };
        });

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = WriteHealthCheckResponse
        });

        app.MapHealthChecks("/alive", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live"),
            ResponseWriter = WriteHealthCheckResponse
        });

        return app;
    }

    private static IHostApplicationBuilder ConfigureSerilog(this IHostApplicationBuilder builder)
    {
        var serviceName = builder.Environment.ApplicationName;

        var configuration = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ServiceName", serviceName)
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithSpan();

        if (builder.Environment.IsDevelopment())
        {
            configuration.WriteTo.Console();
        }
        else
        {
            configuration.WriteTo.Console(new Serilog.Formatting.Compact.CompactJsonFormatter());
        }

        var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];

        if (!string.IsNullOrWhiteSpace(otlpEndpoint))
        {
            configuration.WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = otlpEndpoint;
                options.Protocol = OtlpProtocol.Grpc;
                options.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = serviceName
                };
            });
        }

        Log.Logger = configuration.CreateLogger();
        builder.Services.AddSerilog();

        return builder;
    }

    private static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter("BauDoku.Projects", "BauDoku.Documentation", "BauDoku.Sync");
            })
            .WithTracing(tracing =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    tracing.SetSampler<AlwaysOnSampler>();
                }
                else
                {
                    tracing.SetSampler(new TraceIdRatioBasedSampler(0.1));
                }

                tracing.AddSource(builder.Environment.ApplicationName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();
            });

        AddOpenTelemetryExporters(builder);

        return builder;
    }

    private static void AddOpenTelemetryExporters(IHostApplicationBuilder builder)
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }
    }

    private static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder, Action<IHealthChecksBuilder>? configureHealthChecks)
    {
        var healthChecks = builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        configureHealthChecks?.Invoke(healthChecks);

        var rabbitConnection = builder.Configuration.GetConnectionString("rabbitmq");
        if (!string.IsNullOrWhiteSpace(rabbitConnection))
        {
            var rabbitUri = new Uri(rabbitConnection);
            var lazyConnection = new Lazy<Task<IConnection>>(() => new ConnectionFactory { Uri = rabbitUri }.CreateConnectionAsync());

            healthChecks.AddRabbitMQ(sp => lazyConnection.Value, name: "rabbitmq", failureStatus: HealthStatus.Degraded, tags: ["ready"]);
        }

        var redisConnection = builder.Configuration.GetConnectionString("redis");
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            healthChecks.AddRedis(redisConnection, name: "redis", failureStatus: HealthStatus.Degraded, tags: ["ready"]);
        }

        return builder;
    }

    private static Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var result = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            entries = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds,
                description = e.Value.Description,
                exception = e.Value.Exception?.Message,
                tags = e.Value.Tags
            })
        };

        return context.Response.WriteAsJsonAsync(result, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
    }
}
