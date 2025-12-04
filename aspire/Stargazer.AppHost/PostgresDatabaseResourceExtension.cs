using System.Runtime.InteropServices;

namespace Stargazer.AppHost;

public static class PostgresDatabaseResourceExtension
{
    private static IResourceBuilder<PostgresServerResource> Postgres { get; set; }

    public static IResourceBuilder<PostgresDatabaseResource> CreatePostgresDatabaseResource(
        this IDistributedApplicationBuilder builder)
    {
        string tag = "15-alpine";
        string pgadmin4Tag = "9.4.0";
        Architecture architecture = RuntimeInformation.ProcessArchitecture;
        if (architecture == Architecture.Arm
            || architecture == Architecture.Arm64)
        {
            tag =  "15-alpine-arm";
            pgadmin4Tag = "9.4.0-arm64";
        }
        var username = builder.AddParameter("postgres-uid", "postgres");
        var password = builder.AddParameter("postgres-pwd", "123456");
        Postgres = builder.AddPostgres("postgres", username, password, 5432)
            .WithContainerName("postgres")
            .WithImageRegistry("ccr.ccs.tencentyun.com")
            .WithImage("stargazer/postgres",tag)
            .WithPgAdmin(configureContainer =>
            {
                configureContainer.WithHostPort(8080);
                configureContainer.WithImageRegistry("ccr.ccs.tencentyun.com")
                    .WithImage("stargazer/pgadmin4", pgadmin4Tag);
            })
            .WithBindMount("../../volumes/postgres/data", "/var/lib/postgresql/data");
        return Postgres.AddDatabase("postgresql");
    }

    public static IResourceBuilder<PostgresServerResource> GetPostgresServerResource(this IResourceBuilder<PostgresDatabaseResource> builder)
    {
        return Postgres;
    }
}