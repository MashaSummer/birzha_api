using Confluent.Kafka;
using ProductGrpc;
using ProductMicroservice.Definitions.Base;
using ProductMicroservice.Definitions.Kafka.Handlers;
using ProductMicroservice.Definitions.Kafka.Models;
using ProductMicroservice.Domain.EventsBase;
using ProductMicroservice.Infrastructure.Kafka.Config;

namespace ProductMicroservice.Definitions.Kafka;

public class KafkaDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var isEnableKafka = bool.Parse(configuration["Kafka:IsEnable"]);
        if (!isEnableKafka)
            return;

        var producerConfig = configuration.GetSection("Kafka:ProducerConfig").Get<KafkaProducerConfig>();

        services.AddKafkaProducer<Null, ProductCreatedEvent>(producerConfig);
    }
}