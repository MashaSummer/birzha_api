using Facade.Web.Definitions.Base;

namespace Facade.Web.Auth;

public class AuthDefinition : AppDefinition
{
    public override void ConfigureApplication(WebApplication app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(endpoint => endpoint.MapGrpcService<AuthService>());
    }

    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration) => services.AddGrpc();
}