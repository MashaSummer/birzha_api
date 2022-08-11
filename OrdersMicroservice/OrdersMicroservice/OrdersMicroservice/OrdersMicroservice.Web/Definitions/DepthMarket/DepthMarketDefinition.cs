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
            services.AddTransient<IDepthMarketService, DepthMarketService>();
            
        }
    }
}
