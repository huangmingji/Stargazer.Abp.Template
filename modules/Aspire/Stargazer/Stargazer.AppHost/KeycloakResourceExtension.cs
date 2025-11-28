namespace Stargazer.AppHost;

public static class KeycloakResourceExtension
{
    public static IResourceBuilder<KeycloakResource> CreateKeycloakResource(
        this IDistributedApplicationBuilder builder)
    {
        var username = builder.AddParameter("username", "admin");
        var password = builder.AddParameter("password", "admin");
        return builder.AddKeycloak("keycloak", 8080, username, password)
            .WithEnvironment("KC_BOOTSTRAP_ADMIN_USERNAME", "admin")
            .WithEnvironment("KC_BOOTSTRAP_ADMIN_PASSWORD", "admin")
            .WithEnvironment("KC_DB", "postgres")
            .WithEnvironment("KC_DB_URL_HOST", "postgres")
            .WithEnvironment("KC_DB_URL_DATABASE", "postgres")
            .WithEnvironment("KC_DB_USERNAME", "keycloak")
            .WithEnvironment("KC_DB_PASSWORD", "password")
            .WithDataBindMount("../../../volumes/keycloak/data");
    }
}