using BalanceMicroservice.Web.Definitions.Base;
using BalanceMicroservice.Web.Endpoints.BalanceEndpoints.ViewModels;
using Grpc.AspNetCore.Server;

namespace BalanceMicroservice.Web.GrpcService
{
    public class GrpcDefinition : AppDefinition
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpc(options =>
            {
                options.IgnoreUnknownServices = true;
                options.EnableDetailedErrors = true;
            });
        }

        public override void ConfigureApplication(WebApplication app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<QueryBalanceController>();
                endpoints.MapGrpcService<CommandBalanceService>();
            });
        }
    }
}