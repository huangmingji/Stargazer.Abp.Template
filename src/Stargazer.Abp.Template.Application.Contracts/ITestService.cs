using Volo.Abp.Application.Services;

namespace Stargazer.Abp.Template.Application.Contracts;

public interface ITestService : IApplicationService
{ 
    Task CreateAsync();
}