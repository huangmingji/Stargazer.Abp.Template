using System.Runtime.InteropServices;

namespace Stargazer.AppHost;

public static class KeycloakResourceExtension
{
    public static IResourceBuilder<KeycloakResource> CreateKeycloakResource(
        this IDistributedApplicationBuilder builder)
    {
        string tag = "26.4.6";
        Architecture architecture = RuntimeInformation.ProcessArchitecture;
        if (architecture == Architecture.Arm
            || architecture == Architecture.Arm64)
        {
            tag =  "26.4.6-arm64";
        }
        // var username = builder.AddParameter("username", "admin");
        // var password = builder.AddParameter("password", "admin");
        return builder.AddKeycloak("keycloak", 8081)
            .WithContainerName("keycloak")
            .WithImageRegistry("ccr.ccs.tencentyun.com")
            .WithImage("stargazer/keycloak",tag)
            .WithEnvironment("KC_BOOTSTRAP_ADMIN_USERNAME", "admin")
            .WithEnvironment("KC_BOOTSTRAP_ADMIN_PASSWORD", "admin")
            .WithEnvironment("KC_DB", "postgres")
            .WithEnvironment("KC_DB_URL_HOST", "postgres")
            .WithEnvironment("KC_DB_URL_DATABASE", "keycloak")
            .WithEnvironment("KC_DB_USERNAME", "postgres")
            .WithEnvironment("KC_DB_PASSWORD", "123456")
            .WithDataBindMount("../../volumes/keycloak/data");
    }
}