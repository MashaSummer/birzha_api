using Facade.Domain.Base;
using Facade.Web.Application;
using Facade.Web.Auth;
using Facade.Web.Definitions.Base;
using Facade.Web.GrpcServices.Balance;
using Facade.Web.GrpcServices.Portfolio;
using Facade.Web.GrpcServices.Orders;
using Facade.Web.GrpcServices.Product;

namespace Facade.Web.GrpcServices
{
    public class GrpcDefinition : AppDefinition
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration) 
        {
            services.AddGrpc();
            services.Configure<ServiceUrls>(configuration.GetSection("ServiceUrls"));
            services.AddGrpcReflection();
        }

        public override void ConfigureApplication(WebApplication app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseCors(AppData.PolicyName);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoint => endpoint.MapGrpcService<AuthService>());
            app.UseEndpoints(endpoint => endpoint.MapGrpcService<ProductService>());
            app.UseEndpoints(endpoint => endpoint.MapGrpcService<BalanceService>());
            app.UseEndpoints(endpoint => endpoint.MapGrpcService<PortfolioService>());
            app.UseEndpoints(endpoint => endpoint.MapGrpcService<OrderService>());
            app.MapGrpcReflectionService();
        }
    }
}