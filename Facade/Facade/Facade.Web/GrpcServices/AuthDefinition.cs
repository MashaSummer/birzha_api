using Facade.Web.Definitions.Base;
using ILogger = Grpc.Core.Logging.ILogger;

namespace Facade.Web.GrpcServices;

public class AuthDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<HttpClient>();
        services.AddSingleton(provider =>
            new AuthService(
                provider.GetRequiredService<ILogger<AuthService>>(), 
                configuration.GetConnectionString("authUrl"), 
                provider.GetRequiredService<HttpClient>())
        );
    }
}