// When --grafana is used, allow anonymous OTLP access so the external OTel Collector can forward telemetry
if (args.Contains("--grafana"))
    Environment.SetEnvironmentVariable("DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS", "true");

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImage("postgis/postgis", "17-3.5")
    .WithPgAdmin();

var projectsDb = postgres.AddDatabase("ProjectsDb");
var documentationDb = postgres.AddDatabase("DocumentationDb");
var syncDb = postgres.AddDatabase("SyncDb");

var redis = builder.AddRedis("redis");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

var keycloak = builder.AddContainer("keycloak", "quay.io/keycloak/keycloak", "26.5")
    .WithHttpEndpoint(port: 8080, targetPort: 8080)
    .WithEnvironment("KC_BOOTSTRAP_ADMIN_USERNAME", "admin")
    .WithEnvironment("KC_BOOTSTRAP_ADMIN_PASSWORD", "admin")
    .WithEnvironment("KC_HTTP_RELATIVE_PATH", "/")
    .WithBindMount("./keycloak", "/opt/keycloak/data/import")
    .WithArgs("start-dev", "--import-realm");

var migrationRunner = builder.AddProject("migration-runner", @"..\..\Tools\BauDoku.MigrationRunner\BauDoku.MigrationRunner.csproj")
    .WithReference(projectsDb)
    .WithReference(documentationDb)
    .WithReference(syncDb)
    .WaitFor(projectsDb)
    .WaitFor(documentationDb)
    .WaitFor(syncDb);

var projectsApi = builder.AddProject("projects-api", @"..\..\Services\Projects\BauDoku.Projects.Api\BauDoku.Projects.Api.csproj")
    .WithReference(projectsDb)
    .WithReference(rabbitmq)
    .WaitFor(projectsDb)
    .WaitFor(rabbitmq)
    .WaitFor(keycloak);

var documentationApi = builder.AddProject("documentation-api", @"..\..\Services\Documentation\BauDoku.Documentation.Api\BauDoku.Documentation.Api.csproj")
    .WithReference(documentationDb)
    .WithReference(rabbitmq)
    .WaitFor(documentationDb)
    .WaitFor(rabbitmq)
    .WaitFor(keycloak);

var syncApi = builder.AddProject("sync-api", @"..\..\Services\Sync\BauDoku.Sync.Api\BauDoku.Sync.Api.csproj")
    .WithReference(syncDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(syncDb)
    .WaitFor(redis)
    .WaitFor(rabbitmq)
    .WaitFor(keycloak);

var apiGateway = builder.AddProject("api-gateway", @"..\..\ApiGateway\BauDoku.ApiGateway\BauDoku.ApiGateway.csproj")
    .WithReference(projectsApi)
    .WithReference(documentationApi)
    .WithReference(syncApi)
    .WithExternalHttpEndpoints()
    .WaitFor(projectsApi)
    .WaitFor(documentationApi)
    .WaitFor(syncApi);

builder.AddExecutable("web", "npm", @"..\..\..\..\src\frontend\web", "run", "dev")
    .WithHttpEndpoint(port: 5173, isProxied: false)
    .WaitFor(apiGateway)
    .WaitFor(keycloak);

// Optional Grafana observability stack: dotnet run -- --grafana
if (args.Contains("--grafana"))
{
    var loki = builder.AddContainer("loki", "grafana/loki", "3.4.2")
        .WithHttpEndpoint(targetPort: 3100, name: "http", isProxied: false)
        .WithBindMount("../../../../monitoring/loki.yml", "/etc/loki/loki.yml")
        .WithVolume("loki-data", "/loki")
        .WithArgs("-config.file=/etc/loki/loki.yml");

    var prometheus = builder.AddContainer("prometheus", "prom/prometheus", "v3.2.1")
        .WithHttpEndpoint(port: 9090, targetPort: 9090, name: "http", isProxied: false)
        .WithBindMount("../../../../monitoring/prometheus.yml", "/etc/prometheus/prometheus.yml")
        .WithBindMount("../../../../grafana/alerting", "/etc/prometheus/rules")
        .WithArgs("--config.file=/etc/prometheus/prometheus.yml",
                  "--storage.tsdb.retention.time=24h",
                  "--enable-feature=exemplar-storage",
                  "--enable-feature=remote-write-receiver",
                  "--web.enable-lifecycle");

    var tempo = builder.AddContainer("tempo", "grafana/tempo", "2.7.1")
        .WithHttpEndpoint(targetPort: 3200, name: "http", isProxied: false)
        .WithBindMount("../../../../monitoring/tempo.yml", "/etc/tempo/tempo.yml")
        .WithArgs("-config.file=/etc/tempo/tempo.yml")
        .WaitFor(prometheus);

    var otelCollector = builder.AddContainer("otel-collector", "otel/opentelemetry-collector-contrib", "0.120.0")
        .WithHttpEndpoint(port: 4317, targetPort: 4317, name: "grpc", isProxied: false)
        .WithHttpEndpoint(port: 4318, targetPort: 4318, name: "http", isProxied: false)
        .WithHttpEndpoint(targetPort: 8889, name: "metrics", isProxied: false)
        .WithBindMount("../../../../monitoring/otel-collector-aspire.yml", "/etc/otelcol/config.yml")
        .WithArgs("--config=/etc/otelcol/config.yml")
        .WaitFor(tempo)
        .WaitFor(loki);

    builder.AddContainer("grafana", "grafana/grafana", "11.5.2")
        .WithHttpEndpoint(port: 3000, targetPort: 3000, name: "http", isProxied: false)
        .WithEnvironment("GF_AUTH_ANONYMOUS_ENABLED", "true")
        .WithEnvironment("GF_AUTH_ANONYMOUS_ORG_ROLE", "Admin")
        .WithEnvironment("GF_AUTH_DISABLE_LOGIN_FORM", "true")
        .WithEnvironment("GF_FEATURE_TOGGLES_ENABLE", "traceToMetrics tempoServiceGraph tempoSearch tempoApmTable")
        .WithBindMount("../../../../grafana/provisioning", "/etc/grafana/provisioning")
        .WithBindMount("../../../../grafana/dashboards", "/var/lib/grafana/dashboards")
        .WaitFor(prometheus)
        .WaitFor(tempo)
        .WaitFor(loki);

    // Redirect telemetry from .NET services to OTel Collector (fans out to Aspire Dashboard + Grafana stack)
    projectsApi.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4317");
    documentationApi.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4317");
    syncApi.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4317");
    apiGateway.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4317");
}

builder.Build().Run();
