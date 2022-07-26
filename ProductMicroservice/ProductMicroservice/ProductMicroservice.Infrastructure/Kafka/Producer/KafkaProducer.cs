using Calabonga.OperationResults;
using Confluent.Kafka;
using ProductMicroservice.EventsBase;
using ProductMicroservice.Infrastructure.Kafka.Config;

namespace ProductMicroservice.Infrastructure.Kafka.Producer;

public class KafkaProducer<Tk, Tv> : IEventProducer<Tk, Tv>, IDisposable
{
    private readonly IProducer<Tk, Tv> _producer;
    private readonly string _topic;

    public KafkaProducer(KafkaProducerConfig config, IProducer<Tk, Tv> producer)
    {
        _producer = producer;
        _topic = config.Topic;
    }

    public async Task<OperationResult<bool>> ProduceAsync(Tk key, Tv value)
    {
        var result = new OperationResult<bool>();
        try
        {
            await _producer.ProduceAsync(_topic, new Message<Tk, Tv>()
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