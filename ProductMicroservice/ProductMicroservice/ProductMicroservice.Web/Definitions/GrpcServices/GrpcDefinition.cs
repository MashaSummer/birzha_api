using ProductMicroservice.Definitions.Base;

namespace ProductMicroservice.Definitions.GrpcServices;

public class GrpcDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();
        services.AddGrpcReflection();

    }

    public override void ConfigureApplication(WebApplication app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(endpoint => endpoint.MapGrpcService<ProductService>());

        app.MapGrpcReflectionService();
    }
}