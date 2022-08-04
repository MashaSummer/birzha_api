using Calabonga.OperationResults;
using Confluent.Kafka;
using OrdersEvent;
using OrdersMicroservice.Definitions.Mongodb.Models;
using OrdersMicroservice.Domain.DbBase;
using OrdersMicroservice.Domain.EventsBase;

namespace OrdersMicroservice.Definitions.Kafka.Handlers;

public class ValidationHandler : IEventHandler<Null, OrderValidationEvent>
{
    private readonly IDbWorker<OrderModel> _dbWorker;

    private readonly ILogger<ValidationHandler> _logger;
    //private readonly DeepMarketService _deepMarketService;
    public ValidationHandler(IDbWorker<OrderModel> dbWorker, ILogger<ValidationHandler> logger
        /*DeepMarketService deepMarketService*/)
    {
        _dbWorker = dbWorker;
        _logger = logger;
        //_deepMarketService = deepMarketService;
    }

    public void Process(Message<Null, OrderValidationEvent> message)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> ProcessAsync(Message<Null, OrderValidationEvent> message)
    {
        // TODO change status of order in db
        _logger.LogInformation($"Get {nameof(OrderValidationEvent)} event");

        return Task.FromResult(new OperationResult<bool>()
        {
            Result = true
        });
    }
}