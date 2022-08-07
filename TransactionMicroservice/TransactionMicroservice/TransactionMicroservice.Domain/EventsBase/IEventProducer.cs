using Calabonga.OperationResults;

namespace TransactionMicroservice.EventsBase;

public interface IEventProducer<Tk, Tv>
{
    Task<OperationResult<bool>> ProduceAsync(Tk key, Tv value);
}