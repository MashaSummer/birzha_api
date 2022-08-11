using Confluent.Kafka;
using PortfolioMicroService.Definitions.Base;
using PortfolioMicroService.Domain.EventsBase;

namespace PortfolioMicroService.Definitions.Kafka.Handlers
{
    public class HandlersDefinition: AppDefinition
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEventHandler<Null, ProductAddEvent>, AddUserHandler>();
        }
    }
}
