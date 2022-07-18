using AuthMicroservice.Web.Application.Services;
using AuthMicroservice.Web.Definitions.Base;
using AuthMicroservice.Web.Definitions.Identity;

namespace AuthMicroservice.Web.Definitions.DependencyContainer;

/// <summary>
/// Dependency container definition
/// </summary>
public class ContainerDefinition: AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<ApplicationUserClaimsPrincipalFactory>();
    }
}