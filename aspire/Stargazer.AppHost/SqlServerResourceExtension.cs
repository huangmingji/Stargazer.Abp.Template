namespace Stargazer.AppHost;

public static class SqlServerResourceExtension
{
    private static IResourceBuilder<SqlServerServerResource> SqlServer { get; set; }

    public static IResourceBuilder<SqlServerDatabaseResource> CreateSqlServerDatabaseResource(
        this IDistributedApplicationBuilder builder)
    {
        var password = builder.AddParameter("password", "password");
        SqlServer = builder.AddSqlServer("sqlserver", password).WithDataBindMount("../../volumes/sqlserver/data");
        return SqlServer.AddDatabase("sqlserver-database");
    }

    public static IResourceBuilder<SqlServerServerResource> GetSqlServerServerResource(
        this IResourceBuilder<SqlServerDatabaseResource> builder)
    {
        return SqlServer;
    }
}