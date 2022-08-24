using Calabonga.OperationResults;

namespace NewPortfolioMicroservice.EventsBase;

public interface IEventProducer<Tk, Tv>
{
    Task<OperationResult<bool>> ProduceAsync(Tk key, Tv value);
}