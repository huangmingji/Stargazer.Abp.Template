using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Stargazer.Abp.Template.DbMigrations
{
    public class DbMigrationService : ITransientDependency
    {
        private IServiceProvider _serviceProvider;
        public DbMigrationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            // var dbMigrationsDbContext = _serviceProvider.GetService<DbMigrationsDbContext>();
            // if(dbMigrationsDbContext != null)
            // {
            //     await dbMigrationsDbContext.Database.MigrateAsync();
            // }
            //
            // await InitDataAsync();
        }
    }
}