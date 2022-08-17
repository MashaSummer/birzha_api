using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Google.Protobuf;
using TransactionMicroservice.Domain.EventsBase;
using TransactionMicroservice.EventsBase;
using TransactionMicroservice.Infrastructure.Kafka.Config;
using TransactionMicroservice.Infrastructure.Kafka.Consumer;
using TransactionMicroservice.Infrastructure.Kafka.Producer;
using TransactionMicroservice.Infrastructure.Kafka.Protobuf;

namespace TransactionMicroservice.Definitions.Kafka;

public static class KafkaExtension
{
    public static IServiceCollection AddKafkaProducer<TKeyey, TValue>(this IServiceCollection services,
        KafkaProducerConfig producerConfig, ISerializer<TValue> serializer)
    {
        services.AddTransient<IProducer<TKeyey, TValue>>(provider =>
            new ProducerBuilder<TKeyey, TValue>(producerConfig.ProducerConfig).SetValueSerializer(serializer).Build());

        services.AddSingleton<IEventProducer<TKeyey, TValue>>(provider =>
            new KafkaProducer<TKeyey, TValue>(producerConfig, provider.GetRequiredService<IProducer<TKeyey, TValue>>()));

        return services;
    }

    public static IServiceCollection AddKafkaProducer<TKeyey, TValue>(this IServiceCollection services,
        KafkaProducerConfig producerConfig) where TValue : IMessage<TValue>, new()
    {
        services.AddTransient<IProducer<TKeyey, TValue>>(provider =>
            new ProducerBuilder<TKeyey, TValue>(producerConfig.ProducerConfig)
                .SetValueSerializer(new ProtobufSerializer<TValue>()).Build());

        services.AddSingleton<IEventProducer<TKeyey, TValue>>(provider =>
            new KafkaProducer<TKeyey, TValue>(producerConfig, provider.GetRequiredService<IProducer<TKeyey, TValue>>()));

        return services;
    }


    public static IServiceCollection AddKafkaConsumer<TKey, TValue>(this IServiceCollection services,
        KafkaConsumerConfig consumerConfig, IDeserializer<TValue> deserializer, IEventHandler<TKey, TValue> handler)
    {
        services.AddTransient<IConsumer<TKey, TValue>>(provider =>
            new ConsumerBuilder<TKey, TValue>(consumerConfig.ConsumerConfig).SetValueDeserializer(deserializer).Build());

        services.AddSingleton(handler);

        services.AddHostedService<KafkaConsumer<TKey, TValue>>(provider =>
            new KafkaConsumer<TKey, TValue>(consumerConfig, provider.GetRequiredService<IConsumer<TKey, TValue>>(), handler));

        return services;
    }


    public static IServiceCollection AddKafkaConsumer<TKeyey, TValue>(this IServiceCollection services,
        KafkaConsumerConfig consumerConfig, IEventHandler<TKeyey, TValue> handler)
        where TValue : class, IMessage<TValue>, new()
    {
        services.AddTransient<IConsumer<TKeyey, TValue>>(provider =>
            new ConsumerBuilder<TKeyey, TValue>(consumerConfig.ConsumerConfig)
                .SetValueDeserializer(new ProtobufDeserializer<TValue>()).Build());

        services.AddSingleton(handler);


        services.AddHostedService<KafkaConsumer<TKeyey, TValue>>(provider =>
            new KafkaConsumer<TKeyey, TValue>(consumerConfig, provider.GetRequiredService<IConsumer<TKeyey, TValue>>(),
                handler));

        return services;
    }


    public static IServiceCollection AddKafkaConsumer<TKeyey, TValue>(this IServiceCollection services,
        KafkaConsumerConfig consumerConfig) where TValue : class, IMessage<TValue>, new()
    {
        services.AddTransient<IConsumer<TKeyey, TValue>>(provider =>
            new ConsumerBuilder<TKeyey, TValue>(consumerConfig.ConsumerConfig)
                .SetValueDeserializer(new ProtobufDeserializer<TValue>()).Build());

        services.AddHostedService<KafkaConsumer<TKeyey, TValue>>(provider =>
            new KafkaConsumer<TKeyey, TValue>(consumerConfig, provider.GetRequiredService<IConsumer<TKeyey, TValue>>(),
                provider.GetRequiredService<IEventHandler<TKeyey, TValue>>()));

        return services;
    }
}