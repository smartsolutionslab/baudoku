var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var projectsDb = postgres.AddDatabase("projects-db");
var documentationDb = postgres.AddDatabase("documentation-db");
var syncDb = postgres.AddDatabase("sync-db");

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

var projectsApi = builder.AddProject("projects-api", @"..\..\Services\Projects\BauDoku.Projects.Api\BauDoku.Projects.Api.csproj")
    .WithReference(projectsDb)
    .WithReference(rabbitmq);

var documentationApi = builder.AddProject("documentation-api", @"..\..\Services\Documentation\BauDoku.Documentation.Api\BauDoku.Documentation.Api.csproj")
    .WithReference(documentationDb)
    .WithReference(rabbitmq);

var syncApi = builder.AddProject("sync-api", @"..\..\Services\Sync\BauDoku.Sync.Api\BauDoku.Sync.Api.csproj")
    .WithReference(syncDb)
    .WithReference(redis)
    .WithReference(rabbitmq);

builder.AddProject("api-gateway", @"..\..\ApiGateway\BauDoku.ApiGateway\BauDoku.ApiGateway.csproj")
    .WithReference(projectsApi)
    .WithReference(documentationApi)
    .WithReference(syncApi);

builder.Build().Run();
