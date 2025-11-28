using Stargazer.Abp.Account.Application.Contracts;
using Stargazer.Abp.Authentication.JwtBearer.Application.Contracts;
using Stargazer.Abp.ObjectStorage.Application.Contracts;
using Volo.Abp.Application;
using Volo.Abp.FluentValidation;
using Volo.Abp.Modularity;

namespace Stargazer.Abp.Template.Application
{
    [DependsOn(
        typeof(Users.Application.Contracts.ApplicationContractsModule),
        typeof(StargazerAbpAuthenticationJwtBearerApplicationContractsModule),
        typeof(StargazerAbpObjectStorageApplicationContractsModule),
        typeof(AbpFluentValidationModule),
        typeof(AbpDddApplicationContractsModule)
    )]
    public class ApplicationContractsModule : AbpModule
    {
        
    }
}