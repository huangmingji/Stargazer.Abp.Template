namespace Stargazer.AppHost;

public static class OracleResourceExtension
{
    
    private static IResourceBuilder<OracleDatabaseServerResource> Oracle { get; set; }
    
    public static IResourceBuilder<OracleDatabaseResource> CreateOracleDatabaseResource(
        this IDistributedApplicationBuilder builder)
    {
        var password = builder.AddParameter("password", "password");
        Oracle = builder.AddOracle("oracle", password).WithContainerName("oracle").WithDataBindMount("../../volumes/oracle/data");
        return Oracle.AddDatabase("oracle-database");
    }

    public static IResourceBuilder<OracleDatabaseServerResource> GetOracleDatabaseServerResource(
        this IResourceBuilder<OracleDatabaseResource> builder)
    {
        return Oracle;
    }
}