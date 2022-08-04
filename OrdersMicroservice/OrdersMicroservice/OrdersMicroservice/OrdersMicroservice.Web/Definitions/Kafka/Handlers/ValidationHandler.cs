using Calabonga.OperationResults;
using Confluent.Kafka;
using OrdersEvent;
using OrdersMicroservice.Definitions.Mongodb.Models;
using OrdersMicroservice.Domain.DbBase;
using OrdersMicroservice.Domain.EventsBase;
using OrdersMicroservice.Domain.IServices;
using OrdersMicroservice.Web.Definitions.DepthMarket.Services;

namespace OrdersMicroservice.Definitions.Kafka.Handlers;

public class ValidationHandler : IEventHandler<Null, OrderValidationEvent>
{

    private readonly IDbWorker<OrderModel> _dbWorker; // TODO get db worker from DI
    private readonly DepthMarketService _depthMarketService;
    private readonly ILogger<ValidationHandler> _logger;
    public ValidationHandler(IDbWorker<OrderModel> dbWorker,
        DepthMarketService depthMarketService,
        ILogger<ValidationHandler> logger)
    {
        _dbWorker = dbWorker;
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
            await _dbWorker.UpdateRecords(x => x.Id.ToString() == message.Value.OrderId, y => y.Status = OrderStatus.Validated);

            await _depthMarketService.ProcessOrderAsync(message.Value.OrderId);
        }

        _logger.LogInformation($"Get {nameof(OrderValidationEvent)} event");


        return new OperationResult<bool>()
        {
            Result = true
        };
    }
}