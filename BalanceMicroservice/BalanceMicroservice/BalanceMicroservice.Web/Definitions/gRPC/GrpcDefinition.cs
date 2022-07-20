using BalanceMicroservice.Web.Definitions.Base;
using BalanceMicroservice.Web.Endpoints.BalanceEndpoints.ViewModels;

namespace BalanceMicroservice.Web.GrpcService
{
    public class GrpcDefinition : AppDefinition
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddGrpc();

        public override void ConfigureApplication(WebApplication app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<QueryBalanceController>();
                endpoints.MapGrpcService<CommandBalanceController>();
            });
        }
    }
}