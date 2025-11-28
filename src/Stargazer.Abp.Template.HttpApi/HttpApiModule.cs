using Stargazer.Abp.Captcha.HttpApi;
using Stargazer.Abp.ObjectStorage.HttpApi;
using Stargazer.Abp.Template.Application;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Stargazer.Abp.Template.HttpApi
{
    [DependsOn(
        typeof(StargazerAbpCaptchaHttpApiModule),
        typeof(StargazerAbpObjectStorageHttpApiModule),
        typeof(Users.HttpApi.HttpApiModule),
        typeof(ApplicationContractsModule),
        typeof(AbpAspNetCoreMvcModule)
    )]
    public class HttpApiModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
        }
    }
}