using Calabonga.OperationResults;
using Confluent.Kafka;
using OrdersEvent;
using OrdersMicroservice.Definitions.DepthMarket;
using OrdersMicroservice.Definitions.DepthMarket.Services;
using OrdersMicroservice.Definitions.Mongodb.Models;
using OrdersMicroservice.Domain.DbBase;
using OrdersMicroservice.Domain.EventsBase;

namespace OrdersMicroservice.Definitions.Kafka.Handlers;

public class ValidationHandler : IEventHandler<Null, OrderValidationEvent>
{

    private readonly IRepository<OrderModel> _repository;
    private readonly DepthMarketService _depthMarketService;
    private readonly ILogger<ValidationHandler> _logger;
    public ValidationHandler(IRepository<OrderModel> repository,
        DepthMarketService depthMarketService,
        ILogger<ValidationHandler> logger)
    {
        _repository = repository;
        _logger = logger;
        _depthMarketService = depthMarketService;
    }

    public void Process(Message<Null, OrderValidationEvent> message)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationResult<bool>> ProcessAsync(Message<Null, OrderValidationEvent> message)
    {
        if (message.Value.Valid)
        {
            var order = await _repository.GetByIdAsync(message.Value.OrderId);
            order.Result.Status = OrderStatus.Validated;
            
            await _repository.UpdateAsync(order.Result);

            await _depthMarketService.ProcessOrderAsync(message.Value.OrderId);
        }

        _logger.LogInformation($"Get {nameof(OrderValidationEvent)} event");


        return new OperationResult<bool>()
        {
            Result = true
        };
    }
}