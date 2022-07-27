using Facade.Web.Definitions.Base;
using Grpc.Net.Client;

namespace Facade.Web.GrpcBalance;

public class BalanceDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(provider =>
            new BalanceService(
                provider.GetRequiredService<ILogger<BalanceService>>(),
                GrpcChannel.ForAddress(configuration["BalanceServer:Url"])
        ));
    }
}