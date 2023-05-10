using Stargazer.Abp.Template.Application;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace Stargazer.Abp.Template.HttpApi.Client
{
    [DependsOn(
        typeof(ApplicationContractsModule),
        typeof(AbpHttpClientModule))]
    public class HttpApiClientModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            // var configuration = context.Services.GetConfiguration();
            // context.Services.AddHttpClient();
            // context.Services.AddHttpClient("account", config =>
            // {
            //     config.BaseAddress= new Uri(configuration.GetSection("RemoteServices:Account:BaseUrl").Value);
            //     config.DefaultRequestHeaders.Add("Accept", "application/json");
            // });
            // context.Services.AddSingleton<IAccountService, AccountService>();
            context.Services.AddHttpClientProxies(typeof(ApplicationContractsModule).Assembly);
        }
    }
}
