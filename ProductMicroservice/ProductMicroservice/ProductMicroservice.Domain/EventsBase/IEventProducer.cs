using Calabonga.OperationResults;

namespace ProductMicroservice.Domain.EventsBase;

public interface IEventProducer<TKey, TValue>
{
    Task<OperationResult<bool>> ProduceAsync(TKey key, TValue value);
}