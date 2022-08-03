using Confluent.Kafka;
using GrpcServices;
using OrdersMicroservice.Definitions.Base;
using OrdersMicroservice.Definitions.Kafka.Handlers;
using OrdersMicroservice.Definitions.Kafka.Models;
using OrdersMicroservice.EventsBase;
using OrdersMicroservice.Infrastructure.Kafka.Config;

namespace OrdersMicroservice.Definitions.Kafka;

public class KafkaDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var isEnableKafka = bool.Parse(configuration["Kafka:IsEnable"]);
        if (!isEnableKafka)
            return;

        var producerConfig = configuration.GetSection("Kafka:ProducerConfig").Get<KafkaProducerConfig>();

        services.AddKafkaProducer<Null, Request>(producerConfig);

        var consumerConfig = configuration.GetSection("Kafka:ConsumerConfig").Get<KafkaConsumerConfig>();

        services.AddKafkaConsumer<Null, Request>(consumerConfig, new RequestHadler());
    }


    public override void ConfigureApplication(WebApplication app, IWebHostEnvironment env)
    {
        app.MapPost("/kafka/test", async context =>
        {
            var request = new Request()
            {
                Name = "Some name"
            };

            var producer = app.Services.GetRequiredService<IEventProducer<Null, Request>>();

            var result = await producer.ProduceAsync(null, request);

            if (result.Ok)
                Results.Ok();
            else
            {
                Results.BadRequest();
            }
        });
    }
}