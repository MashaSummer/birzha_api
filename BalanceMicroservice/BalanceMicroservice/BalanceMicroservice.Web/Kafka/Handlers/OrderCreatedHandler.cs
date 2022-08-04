using BalanceMicroservice.Domain.EventsBase;
using Calabonga.OperationResults;
using Confluent.Kafka;
using OrdersEvent;
using OrdersMicroservice.EventsBase;

namespace BalanceMicroservice.Web.Kafka.Handlers;

public class OrderCreatedHandler : IEventHandler<Null, OrderCreatedEvent>
{
    private readonly IEventProducer<Null, OrderValidationEvent> _eventProducer;
    private readonly ILogger<OrderCreatedHandler> _logger;

    public OrderCreatedHandler(ILogger<OrderCreatedHandler> logger, IEventProducer<Null, OrderValidationEvent> eventProducer)
    {
        _logger = logger;
        _eventProducer = eventProducer;
    }

    public void Process(Message<Null, OrderCreatedEvent> message) => throw new NotImplementedException();

    public async Task<OperationResult<bool>> ProcessAsync(Message<Null, OrderCreatedEvent> message)
    {
        _logger.LogInformation($"Get {nameof(OrderCreatedEvent)} event");
        await _eventProducer.ProduceAsync(null, new OrderValidationEvent() { OrderId = message.Value.OrderId, Valid = true });

        return new OperationResult<bool>() { Result = true };
    }
}