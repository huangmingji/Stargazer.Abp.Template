using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Stargazer.Abp.Template.EntityFrameworkCore.DbMigrations
{
    [DependsOn(
        typeof(Users.EntityFrameworkCore.DbMigrations.EntityFrameworkCoreDbMigrationsModule),
        typeof(EntityFrameworkCoreModule))]
    public class EntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<DbMigrationsDbContext>(options => {
                options.AddDefaultRepositories(includeAllEntities: true);
            });

            Configure<AbpDbContextOptions>(options =>
            {
                options.UseNpgsql();
            });

            #region 自动迁移数据库

            var  accountDbMigrationsDbContext =  context.Services.BuildServiceProvider().GetService<DbMigrationsDbContext>();
            if (accountDbMigrationsDbContext != null)
            {
                accountDbMigrationsDbContext.Database.Migrate();
            }
            
            #endregion 自动迁移数据库
        }
    }
}