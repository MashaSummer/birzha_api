using Confluent.Kafka;
using OrdersEvent;
using OrdersMicroservice.Definitions.Base;
using OrdersMicroservice.Domain.EventsBase;
using TransactionsEvent;

namespace OrdersMicroservice.Definitions.Kafka.Handlers
{
    public class HandlersDefinitions : AppDefinition
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEventHandler<Null, OrderValidationEvent>, ValidationHandler>();
            services.AddTransient<IEventHandler<Null, TransactionCreatedEvent>, TransactionHandler>();
        }
    }
}
