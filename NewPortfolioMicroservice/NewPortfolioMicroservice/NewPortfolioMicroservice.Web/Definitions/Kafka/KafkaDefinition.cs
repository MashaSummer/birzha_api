using Confluent.Kafka;
using GrpcServices;
using NewPortfolioMicroservice.Definitions.Base;
using NewPortfolioMicroservice.Definitions.Kafka.Handlers;
using NewPortfolioMicroservice.Definitions.Kafka.Models;
using NewPortfolioMicroservice.EventsBase;
using NewPortfolioMicroservice.Infrastructure.Kafka.Config;
using PortfolioMicroService;

namespace NewPortfolioMicroservice.Definitions.Kafka;

public class KafkaDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var isEnableKafka = bool.Parse(configuration["Kafka:IsEnable"]);
        if (!isEnableKafka)
            return;

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