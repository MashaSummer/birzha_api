using Facade.Web.Definitions.Base;

namespace Facade.Web.Definitions.AuthDefinition;

public class AuthenticationDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();
        services.AddSingleton(provider => new AuthenticationService(configuration.GetConnectionString("AuthUrl")));
    }

    public override void ConfigureApplication(WebApplication app, IWebHostEnvironment env) => 
        app.UseEndpoints(endpoints => endpoints.MapGrpcService<AuthenticationService>());
}