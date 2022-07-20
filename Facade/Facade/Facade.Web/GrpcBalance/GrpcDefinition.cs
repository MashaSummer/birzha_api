using Facade.Web.Definitions.Base;

namespace Facade.Web.GrpcBalance;

public class GrpcDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration) => services.AddGrpc();

    public override void ConfigureApplication(WebApplication app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(endpoint => endpoint.MapGrpcService<BalanceService>());
    }
}