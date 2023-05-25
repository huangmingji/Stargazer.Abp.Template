using Microsoft.Extensions.DependencyInjection;
using Stargazer.Abp.Account.Application;
using Stargazer.Abp.Authentication.JwtBearer.Application;
using Stargazer.Abp.ObjectStorage.Application;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Stargazer.Abp.Template.Application
{
    [DependsOn(
        typeof(StargazerAbpAccountApplicationModule),
        typeof(StargazerAbpAuthenticationJwtBearerApplicationModule),
        typeof(StargazerAbpObjectStorageApplicationModule),
        typeof(ApplicationContractsModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule)
    )]
    public class ApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            context.Services.AddAutoMapperObjectMapper<ApplicationModule>();

            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddProfile<ApplicationAutoMapperProfile>(validate: true);
            });
        }
    }
}