namespace Stargazer.AppHost;

public static class MySqlResourceExtension
{
    private static IResourceBuilder<MySqlServerResource> MySql { get; set; }

    public static IResourceBuilder<MySqlDatabaseResource> CreateMySqlDatabaseResource(
        this IDistributedApplicationBuilder builder)
    {
        var password = builder.AddParameter("password", "123456");
        MySql = builder.AddMySql("MySql", password)
            .WithContainerName("mysql")
            .WithDataBindMount("../../volumes/mysql/data");
        return MySql.AddDatabase("mysql-database");
    }

    public static IResourceBuilder<MySqlServerResource> GetMySqlServerResource(this IResourceBuilder<MySqlDatabaseResource> builder)
    {
        return MySql;
    }
}