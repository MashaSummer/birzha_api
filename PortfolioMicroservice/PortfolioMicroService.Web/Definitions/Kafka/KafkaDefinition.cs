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


    }
}