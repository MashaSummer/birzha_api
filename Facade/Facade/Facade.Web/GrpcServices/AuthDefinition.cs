using Facade.Web.Definitions.Base;
using ILogger = Grpc.Core.Logging.ILogger;

namespace Facade.Web.GrpcServices;

public class AuthDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration) => services.AddHttpClient<AuthService>(options => options.BaseAddress = new Uri(configuration.GetConnectionString("authUrl")));
}