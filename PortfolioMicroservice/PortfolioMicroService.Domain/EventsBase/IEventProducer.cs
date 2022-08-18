using Calabonga.OperationResults;

namespace PortfolioMicroService.EventsBase;

public interface IEventProducer<Tk, Tv>
{
    Task<OperationResult<bool>> ProduceAsync(Tk key, Tv value);
}