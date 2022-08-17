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
        _logger.LogInformation($"Get {nameof(OrderCreatedEvent)} event");

        var userBalance = await _database.GetByIdAsync(new Guid(message.Value.Order.InvestorId));

        bool isOrderValid = false;

        if(userBalance.BalanceActive >= message.Value.Order.Price)
        {
            isOrderValid = true;

            userBalance.BalanceActive -= message.Value.Order.Price;
            userBalance.BalanceFrozen += message.Value.Order.Price;
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