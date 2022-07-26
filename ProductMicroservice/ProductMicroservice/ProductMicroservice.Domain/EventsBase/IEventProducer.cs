using Calabonga.OperationResults;

namespace ProductMicroservice.EventsBase;

public interface IEventProducer<Tk, Tv>
{
    Task<OperationResult<bool>> ProduceAsync(Tk key, Tv value);
}