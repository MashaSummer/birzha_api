using Orders;
using OrdersMicroservice.Definitions.Base;
using OrdersMicroservice.Domain.IServices;
using OrdersMicroservice.Web.Definitions.DepthMarket.Services;

namespace OrdersMicroservice.Definitions.DepthMarket
{
    public class DepthMarketDefinition : AppDefinition
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<DepthMarketService>();
            //services.AddGrpc();
        }

        public override void ConfigureApplication(WebApplication app, IWebHostEnvironment env)
        {/*
            app.UseRouting();
            app.UseEndpoints(endpoint => endpoint.MapGrpcService<OrdersService>());*/
        }
    }
}
