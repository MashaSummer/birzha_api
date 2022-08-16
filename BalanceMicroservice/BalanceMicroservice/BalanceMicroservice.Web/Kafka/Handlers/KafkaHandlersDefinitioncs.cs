using BalanceMicroservice.Domain.EventsBase;
using BalanceMicroservice.Web.Definitions.Base;
using Confluent.Kafka;
using OrdersEvent;

namespace BalanceMicroservice.Web.Kafka.Handlers
{
    public class HandlersDefinitions : AppDefinition
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration) => services.AddTransient<IEventHandler<Null, OrderCreatedEvent>, OrderCreatedHandler>();
    }
}
