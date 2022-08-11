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

        var consumerConfig = configuration.GetSection("Kafka:ConsumerConfig").Get<KafkaConsumerConfig>();

        services.AddKafkaConsumer<Null, ChangePortfolioRequest>(consumerConfig);
    }
}