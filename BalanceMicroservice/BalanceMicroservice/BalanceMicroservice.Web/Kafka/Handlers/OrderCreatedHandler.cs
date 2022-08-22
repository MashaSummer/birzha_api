using BalanceMicroservice.Domain.EventsBase;
using BalanceMicroservice.Web.MongoService;
using Calabonga.OperationResults;
using Confluent.Kafka;
using OrdersEvent;
using OrdersMicroservice.EventsBase;

namespace BalanceMicroservice.Web.Kafka.Handlers;

public class OrderCreatedHandler : IEventHandler<Null, OrderCreatedEvent>
{
    private readonly IEventProducer<Null, OrderValidationEvent> _eventProducer;
    private readonly ILogger<OrderCreatedHandler> _logger;
    private readonly MongoBalanceService _database;

    public OrderCreatedHandler(MongoBalanceService mongo, ILogger<OrderCreatedHandler> logger, IEventProducer<Null, OrderValidationEvent> eventProducer)
    {
        _database = mongo;
        _logger = logger;
        _eventProducer = eventProducer;
    }

    public void Process(Message<Null, OrderCreatedEvent> message) => throw new NotImplementedException();

    public async Task<OperationResult<bool>> ProcessAsync(Message<Null, OrderCreatedEvent> message)
    {
        if (message.Value.Order.Type != Orders.OrderType.Bid) { return new OperationResult<bool> { Result = false }; }

        _logger.LogInformation($"Get {nameof(OrderCreatedEvent)} event");

        var userBalance = await _database.GetByIdAsync(new Guid(message.Value.Order.InvestorId));

        bool isOrderValid = false;

        double totalPrice = (double)message.Value.Order.Price * message.Value.Order.Volume / 100;
        if (userBalance.BalanceActive >= totalPrice)
        {
            isOrderValid = true;

            userBalance.BalanceActive -= totalPrice;
            userBalance.BalanceFrozen += totalPrice;
            await _database.UpdateAsync(userBalance);
        }

        await _eventProducer.ProduceAsync(null, 
            new OrderValidationEvent() 
            { 
                OrderId = message.Value.OrderId, 
                Valid = isOrderValid
            });

        return new OperationResult<bool>() { Result = true };
    }
}