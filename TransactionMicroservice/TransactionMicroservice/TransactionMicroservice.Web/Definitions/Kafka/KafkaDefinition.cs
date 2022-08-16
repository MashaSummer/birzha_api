using Confluent.Kafka;
using OrdersEvent;
using TransactionMicroservice.Definitions.Base;
using TransactionMicroservice.Infrastructure.Kafka.Config;
using TransactionsEvent;

namespace TransactionMicroservice.Definitions.Kafka;

public class KafkaDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var isEnableKafka = bool.Parse(configuration["Kafka:IsEnable"]);
        if (!isEnableKafka)
            return;

        var producerConfig = configuration.GetSection("Kafka:TransactionsProducer").Get<KafkaProducerConfig>();

        services.AddKafkaProducer<Null, TransactionCreatedEvent>(producerConfig);

        var consumerConfig = configuration.GetSection("Kafka:CandidatesConsumer").Get<KafkaConsumerConfig>();

        services.AddKafkaConsumer<Null, CandidatesFoundEvent>(consumerConfig);
    }
}