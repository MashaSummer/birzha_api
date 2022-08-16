using Confluent.Kafka;
using Google.Protobuf;
using ProductMicroservice.Domain.EventsBase;
using ProductMicroservice.Infrastructure.Kafka.Config;
using ProductMicroservice.Infrastructure.Kafka.Consumer;
using ProductMicroservice.Infrastructure.Kafka.Producer;
using ProductMicroservice.Infrastructure.Kafka.Protobuf;

namespace ProductMicroservice.Definitions.Kafka;

public static class KafkaExtension
{
    public static IServiceCollection AddKafkaProducer<TKey, TValue>(this IServiceCollection services,
        KafkaProducerConfig producerConfig, ISerializer<TValue> serializer)
    {
        services.AddTransient<IProducer<TKey, TValue>>(provider =>
            new ProducerBuilder<TKey, TValue>(producerConfig.ProducerConfig).SetValueSerializer(serializer).Build());

        services.AddSingleton<IEventProducer<TKey, TValue>>(provider =>
            new KafkaProducer<TKey, TValue>(producerConfig, provider.GetRequiredService<IProducer<TKey, TValue>>()));

        return services;
    }

    public static IServiceCollection AddKafkaProducer<TKey, TValue>(this IServiceCollection services,
        KafkaProducerConfig producerConfig) where TValue : IMessage<TValue>, new()
    {
        services.AddTransient<IProducer<TKey, TValue>>(provider =>
            new ProducerBuilder<TKey, TValue>(producerConfig.ProducerConfig)
                .SetValueSerializer(new ProtobufSerializer<TValue>()).Build());
        
        services.AddSingleton<IEventProducer<TKey, TValue>>(provider =>
            new KafkaProducer<TKey, TValue>(producerConfig, provider.GetRequiredService<IProducer<TKey, TValue>>()));

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


    public static IServiceCollection AddKafkaConsumer<TKey, TValue>(this IServiceCollection services,
        KafkaConsumerConfig consumerConfig, IEventHandler<TKey, TValue> handler) where TValue : class, IMessage<TValue>, new()
    {
        services.AddTransient<IConsumer<TKey, TValue>>(provider =>
            new ConsumerBuilder<TKey, TValue>(consumerConfig.ConsumerConfig)
                .SetValueDeserializer(new ProtobufDeserializer<TValue>()).Build());
        
        services.AddSingleton(handler);
        
        
        services.AddHostedService<KafkaConsumer<TKey, TValue>>(provider =>
            new KafkaConsumer<TKey, TValue>(consumerConfig, provider.GetRequiredService<IConsumer<TKey, TValue>>(), handler));

        return services;
    }
    
    
    public static IServiceCollection AddKafkaConsumer<TKey, TValue>(this IServiceCollection services, 
        KafkaConsumerConfig consumerConfig) where TValue : class, IMessage<TValue>, new()
    {
        services.AddTransient<IConsumer<TKey, TValue>>(provider =>
            new ConsumerBuilder<TKey, TValue>(consumerConfig.ConsumerConfig)
                .SetValueDeserializer(new ProtobufDeserializer<TValue>()).Build());

        services.AddHostedService<KafkaConsumer<TKey, TValue>>(provider =>
            new KafkaConsumer<TKey, TValue>(consumerConfig, provider.GetRequiredService<IConsumer<TKey, TValue>>(),
                provider.GetRequiredService<IEventHandler<TKey, TValue>>()));
        
        return services;
    }
}