using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Stargazer.Abp.Template.EntityFrameworkCore.DbMigrations
{    
    [ConnectionStringName("Default")]
    public class DbMigrationsDbContext: AbpDbContext<DbMigrationsDbContext>
    {
        public DbMigrationsDbContext(DbContextOptions<DbMigrationsDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Configure();
            base.OnModelCreating(builder);
        }

    }
}