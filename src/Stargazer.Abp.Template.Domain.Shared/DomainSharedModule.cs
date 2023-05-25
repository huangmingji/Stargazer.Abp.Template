﻿using Stargazer.Abp.Account.Domain.Shared;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;

namespace Stargazer.Abp.Template.Domain.Shared
{
    [DependsOn(
        typeof(StargazerAbpAccountDomainSharedModule),
        typeof(AbpValidationModule))]
    public class DomainSharedModule : AbpModule
    {
        public DomainSharedModule()
        {
        }
    }
}
