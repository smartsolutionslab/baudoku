namespace BauDoku.Auth.IntegrationTests.Fixtures;

[CollectionDefinition(Name)]
public sealed class KeycloakCollection : ICollectionFixture<KeycloakFixture>
{
    public const string Name = "Keycloak";
}
