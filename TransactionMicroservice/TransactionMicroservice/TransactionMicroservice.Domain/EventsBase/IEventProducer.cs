using Calabonga.OperationResults;

namespace TransactionMicroservice.EventsBase;

public interface IEventProducer<TKey, TValue>
{
    Task<OperationResult<bool>> ProduceAsync(TKey key, TValue value);
}