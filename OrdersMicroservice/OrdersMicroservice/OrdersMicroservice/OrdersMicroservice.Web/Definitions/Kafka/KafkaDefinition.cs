using Confluent.Kafka;
using OrdersEvent;
using OrdersMicroservice.Definitions.Base;
using OrdersMicroservice.Definitions.Kafka.Handlers;
using OrdersMicroservice.Definitions.Kafka.Models;
using OrdersMicroservice.Infrastructure.Kafka.Config;
using TransactionsEvent;

namespace OrdersMicroservice.Definitions.Kafka;

public class KafkaDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {

        var isEnableKafka = bool.Parse(configuration["Kafka:IsEnable"]);
        if (!isEnableKafka)
            return;

        // For "Orders_Created" topic
        var ordersCreatedConfig = configuration.GetSection("Kafka:OrdersCreatedProducer").Get<KafkaProducerConfig>();
        services.AddKafkaProducer<Null, OrderCreatedEvent>(ordersCreatedConfig);


        // For "Orders_Executed" topic
        var ordersExecutedConfig = configuration.GetSection("Kafka:OrdersExecutedProducer").Get<KafkaProducerConfig>();
        services.AddKafkaProducer<Null, OrderExecuteEvent>(ordersExecutedConfig);

        // For "Orders_Candidates" topic
        var ordersCandidatesConfig =
            configuration.GetSection("Kafka:OrdersCandidatesProducer").Get<KafkaProducerConfig>();
        services.AddKafkaProducer<Null, CandidatesFoundEvent>(ordersCandidatesConfig);

        // For "Orders_Validation" topic
        var ordersValidationConfig =
            configuration.GetSection("Kafka:OrdersValidationConsumer").Get<KafkaConsumerConfig>();
        services.AddKafkaConsumer<Null, OrderValidationEvent>(ordersValidationConfig);
        
        // For "Transactions" topic
        var transactionConfig = configuration.GetSection("Kafka:TransactionsConsumer").Get<KafkaConsumerConfig>();
        services.AddKafkaConsumer<Null, TransactionCreatedEvent>(transactionConfig);
    }
}