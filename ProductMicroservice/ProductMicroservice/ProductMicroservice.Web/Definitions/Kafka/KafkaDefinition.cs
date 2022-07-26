using Confluent.Kafka;
using ProductMicroservice.Definitions.Base;
using ProductMicroservice.Definitions.Kafka.Handlers;
using ProductMicroservice.Definitions.Kafka.Models;
using ProductMicroservice.EventsBase;
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

        services.AddKafkaProducer<Null, EventPersonModel>(producerConfig, new PersonSerializer());

        var consumerConfig = configuration.GetSection("Kafka:ConsumerConfig").Get<KafkaConsumerConfig>();

        services.AddKafkaConsumer(consumerConfig, new PersonDeserializer(), new TestHandler());
    }


    public override void ConfigureApplication(WebApplication app, IWebHostEnvironment env)
    {
        app.MapPost("/kafka/test", async context =>
        {
            var person = new EventPersonModel()
            {
                FirstName = context.Request.Query["first_name"],
                LastName = context.Request.Query["last_name"]
            };

            var producer = app.Services.GetRequiredService<IEventProducer<Null, EventPersonModel>>();

            var result = await producer.ProduceAsync(null, person);

            if (result.Ok)
                Results.Ok();
            else
            {
                Results.BadRequest();
            }
        });
    }
}