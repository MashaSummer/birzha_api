using Confluent.Kafka;
using GrpcServices;
using PortfolioMicroService.Definitions.Base;
using PortfolioMicroService.Definitions.Kafka.Handlers;
using PortfolioMicroService.Definitions.Kafka.Models;
using PortfolioMicroService.EventsBase;
using PortfolioMicroService.Infrastructure.Kafka.Config;

namespace PortfolioMicroService.Definitions.Kafka;

public class KafkaDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var isEnableKafka = bool.Parse(configuration["Kafka:IsEnable"]);
        if (!isEnableKafka)
            return;

        var producerConfig = configuration.GetSection("Kafka:ProducerConfig").Get<KafkaProducerConfig>();

        //services.AddKafkaProducer<Null, EventPersonModel>(producerConfig);

        var consumerConfig = configuration.GetSection("Kafka:ConsumerConfig").Get<KafkaConsumerConfig>();

        //services.AddKafkaConsumer<Null, EventPersonModel>(consumerConfig);
    }


    public override void ConfigureApplication(WebApplication app, IWebHostEnvironment env)
    {
        app.MapPost("/kafka/test", async context =>
        {
            var request = new EventPersonModel()
            {
                FirstName = "Some name"
            };

            var producer = app.Services.GetRequiredService<IEventProducer<Null, EventPersonModel>>();

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