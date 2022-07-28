using Confluent.Kafka;
using ProductMicroservice.Domain.EventsBase;
using ProductMicroservice.Infrastructure.Kafka.Config;
using ProductMicroservice.Infrastructure.Kafka.Consumer;
using ProductMicroservice.Infrastructure.Kafka.Producer;

namespace ProductMicroservice.Definitions.Kafka;

public static class KafkaExtension
{
    public static IServiceCollection AddKafkaProducer<TKey, TValue>(this IServiceCollection services,
        KafkaProducerConfig producerConfig, ISerializer<TValue> serializer)
    {
        services.AddSingleton(producerConfig);

        services.AddTransient<IProducer<TKey, TValue>>(provider =>
            new ProducerBuilder<TKey, TValue>(producerConfig.ProducerConfig).SetValueSerializer(serializer).Build());

        services.AddSingleton<IEventProducer<TKey, TValue>, KafkaProducer<TKey, TValue>>();

        return services;
    }


    public static IServiceCollection AddKafkaConsumer<TKey, TValue>(this IServiceCollection services,
        KafkaConsumerConfig consumerConfig, IDeserializer<TValue> deserializer, IEventHandler<TKey, TValue> handler)
    {
        services.AddSingleton(consumerConfig);

        services.AddTransient<IConsumer<TKey, TValue>>(provider =>
            new ConsumerBuilder<TKey, TValue>(consumerConfig.ConsumerConfig).SetValueDeserializer(deserializer).Build());

        services.AddSingleton(handler);

        services.AddHostedService<KafkaConsumer<TKey, TValue>>();

        return services;
    }
}