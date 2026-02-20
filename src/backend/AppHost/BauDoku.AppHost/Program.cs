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

var keycloak = builder.AddContainer("keycloak", "quay.io/keycloak/keycloak", "26.2")
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

builder.AddViteApp("web", @"..\..\..\..\src\frontend\web")
    .WithNpm(install: false)
    .WithReference(apiGateway)
    .WaitFor(keycloak)
    .WithExternalHttpEndpoints();

builder.Build().Run();
