using Facade.Web.Definitions.Base;

namespace Facade.Web.Definitions.AuthDefinition;

public class AuthenticationDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration) => 
        services.AddSingleton(provider => new AuthenticationService(configuration.GetConnectionString("AuthUrl")));
}