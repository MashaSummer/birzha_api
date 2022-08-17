using Calabonga.OperationResults;
using Confluent.Kafka;
using OrdersMicroservice.Domain.EventsBase;
using OrdersMicroservice.Infrastructure.Kafka.Config;

namespace OrdersMicroservice.Infrastructure.Kafka.Producer;

public class KafkaProducer<TKey, TValue> : IEventProducer<TKey, TValue>, IDisposable
{
    private readonly IProducer<TKey, TValue> _producer;
    private readonly string _topic;

    public KafkaProducer(KafkaProducerConfig config, IProducer<TKey, TValue> producer)
    {
        _producer = producer;
        _topic = config.Topic;
    }

    public async Task<OperationResult<bool>> ProduceAsync(TKey key, TValue value)
    {
        var result = new OperationResult<bool>();
        try
        {
            await _producer.ProduceAsync(_topic, new Message<TKey, TValue>()
            {
                Key = key,
                Value = value
            });
            result.Result = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            result.AddError(e.Message);
        }

        return result;
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}