using OrdersMicroservice.Definitions.Base;


namespace OrdersMicroservice.Definitions.DepthMarket
{
    public class DepthMarketDefinition : AppDefinition
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<DepthMarketService>();
            
        }
    }
}
