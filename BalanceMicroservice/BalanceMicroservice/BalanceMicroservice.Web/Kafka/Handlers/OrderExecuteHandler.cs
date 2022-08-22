using BalanceMicroservice.Domain.EventsBase;
using BalanceMicroservice.Web.MongoService;
using Calabonga.OperationResults;
using Confluent.Kafka;
using OrdersEvent;
using OrdersMicroservice.EventsBase;

namespace BalanceMicroservice.Web.Kafka.Handlers;

public class OrderExecuteHandler : IEventHandler<Null, OrderExecuteEvent>
{
    private readonly ILogger<OrderExecuteHandler> _logger;
    private readonly MongoBalanceService _database;

    public OrderExecuteHandler(MongoBalanceService mongo, ILogger<OrderExecuteHandler> logger)
    {
        _database = mongo;
        _logger = logger;
    }

    public void Process(Message<Null, OrderExecuteEvent> message) => throw new NotImplementedException();

    public async Task<OperationResult<bool>> ProcessAsync(Message<Null, OrderExecuteEvent> message)
    {
        double price = (double)message.Value.Price * message.Value.Volume / 100;

        var bidUser = await _database.GetByIdAsync(new Guid(message.Value.BidInvestorId));
        var askUser = await _database.GetByIdAsync(new Guid(message.Value.AskInvestorId));

        bidUser.BalanceFrozen -= price;
        askUser.BalanceActive += price;

        await _database.UpdateAsync(bidUser);
        await _database.UpdateAsync(askUser);

        return new OperationResult<bool> { Result = true };
    }
}