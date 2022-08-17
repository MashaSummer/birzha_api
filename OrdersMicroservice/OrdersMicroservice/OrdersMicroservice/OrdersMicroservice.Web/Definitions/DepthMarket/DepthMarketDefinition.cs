using OrdersMicroservice.Definitions.Base;
using OrdersMicroservice.Definitions.DepthMarket.Services;

namespace OrdersMicroservice.Definitions.DepthMarket;

public class DepthMarketDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<DepthMarketService>();
        services.AddSingleton<DepthMarketSearchService>();
        
    }
}

