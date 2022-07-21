
using Facade.Web.Definitions.Base;
using ILogger = Grpc.Core.Logging.ILogger;
using Grpc.Net.Client;
namespace Facade.Web.GrpcBalance;

public class AuthDefinition : AppDefinition
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