using BalanceMicroservice.Infrastructure.Kafka.Config;
using BalanceMicroservice.Web.Definitions.Base;
using Confluent.Kafka;
using OrdersEvent;
using OrdersMicroservice.EventsBase;

namespace BalanceMicroservice.Web.Kafka;

public class KafkaDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {

        var isEnableKafka = bool.Parse(configuration["Kafka:IsEnable"]);
        if (!isEnableKafka)
        {
            return;
        }
        
        // For "Orders_Created" topic
        var ordersCreatedConfig = configuration.GetSection("Kafka:OrdersCreatedConsumer").Get<KafkaConsumerConfig>();
        services.AddKafkaConsumer<Null, OrderCreatedEvent>(ordersCreatedConfig);
        
        // For "Orders_Validation" topic
        var ordersValidationConfig = configuration.GetSection("Kafka:ValidationProducer").Get<KafkaProducerConfig>();
        services.AddKafkaProducer<Null, OrderValidationEvent>(ordersValidationConfig);
    }
}