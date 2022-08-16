using Confluent.Kafka;
using OrdersEvent;
using TransactionMicroservice.Definitions.Base;
using TransactionMicroservice.Domain.EventsBase;

namespace TransactionMicroservice.Definitions.Kafka.Handlers;

public class HandlersDefinitions : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IEventHandler<Null, CandidatesFoundEvent>, CandidatesHandler>();
    }
}