using Calabonga.OperationResults;

namespace OrdersMicroservice.EventsBase;

public interface IEventProducer<Tk, Tv>
{
    Task<OperationResult<bool>> ProduceAsync(Tk key, Tv value);
}