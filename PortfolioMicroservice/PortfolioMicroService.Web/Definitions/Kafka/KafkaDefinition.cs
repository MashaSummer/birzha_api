using Confluent.Kafka;
using PortfolioMicroService.Definitions.Base;
using PortfolioMicroService.Infrastructure.Kafka.Config;

namespace PortfolioMicroService.Definitions.Kafka;

public class KafkaDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var isEnableKafka = bool.Parse(configuration["Kafka:IsEnable"]);
        if (!isEnableKafka)
            return;

        //var producerConfig = configuration.GetSection("Kafka:ProducerConfig").Get<KafkaProducerConfig>();

        //services.AddKafkaProducer<Null, /*proto*/>(producerConfig);

        var consumerConfigProductAdd = configuration.GetSection("Kafka:ConsumerConfigProductAdd").Get<KafkaConsumerConfig>();
        services.AddKafkaConsumer<Null, ProductAddEvent>(consumerConfigProductAdd);

        var consumerConfigAuthRegister = configuration.GetSection("Kafka:ConsumerConfigAuthRegister").Get<KafkaConsumerConfig>();
        services.AddKafkaConsumer<Null, UserRegisterEvent>(consumerConfigAuthRegister);

        var consumerConfigOrdersExecuted = configuration.GetSection("Kafka:ConsumerConfigOrdersExecuted").Get<KafkaConsumerConfig>();
        services.AddKafkaConsumer<Null, OrderExecuteEvent>(consumerConfigOrdersExecuted);
        var consumerConfigOrdersCreated = configuration.GetSection("Kafka:ConsumerConfigOrdersCreate").Get<KafkaConsumerConfig>();
        services.AddKafkaConsumer<Null, OrderCreatedEvent>(consumerConfigOrdersCreated);
        var producerConfigOrdersValidate = configuration.GetSection("Kafka:ProducerConfigOrdersValidate").Get<KafkaProducerConfig>();
        services.AddKafkaProducer<Null, OrderValidationEvent>(producerConfigOrdersValidate);
    }
}