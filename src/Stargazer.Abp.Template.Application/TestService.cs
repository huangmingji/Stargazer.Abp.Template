using Stargazer.Abp.Template.Application.Contracts;
using Volo.Abp.Application.Services;

namespace Stargazer.Abp.Template.Application;

public class TestService: ApplicationService, ITestService
{
    public Task CreateAsync()
    {
        throw new NotImplementedException();
    }
}