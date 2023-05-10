using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Stargazer.Abp.Template.EntityFrameworkCore.DbMigrations
{
    public class DbMigrationsDbContextFactory: IDesignTimeDbContextFactory<DbMigrationsDbContext>
    {
        public DbMigrationsDbContext CreateDbContext(string[] args)
        {
            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<DbMigrationsDbContext>()
                .UseNpgsql(configuration.GetConnectionString("Default"));

            return new DbMigrationsDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}