using Calabonga.OperationResults;
using Confluent.Kafka;
using Newtonsoft.Json.Serialization;
using OrdersEvent;
using OrdersMicroservice.Definitions.Mongodb.Models;
using OrdersMicroservice.Domain.DbBase;
using OrdersMicroservice.Domain.EventsBase;
using TransactionsEvent;

namespace OrdersMicroservice.Definitions.Kafka.Handlers;

public class TransactionHandler : IEventHandler<Null, TransactionCreatedEvent>
{
    private readonly IRepository<OrderModel> _repository;
    private readonly IEventProducer<Null, OrderExecuteEvent> _eventProducer;
    private readonly ILogger<TransactionHandler> _logger;

    public TransactionHandler(IRepository<OrderModel> repository, IEventProducer<Null, OrderExecuteEvent> eventProducer,
        ILogger<TransactionHandler> logger)
    {
        _repository = repository;
        _eventProducer = eventProducer;
        _logger = logger;
    }

    public void Process(Message<Null, TransactionCreatedEvent> message)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationResult<bool>> ProcessAsync(Message<Null, TransactionCreatedEvent> message)
    {
        var result = new OperationResult<bool>();
        
        var messageValue = message.Value;

        var askOrderResult = await _repository.GetByIdAsync(messageValue.AskOrder.OrderId);
        if (!askOrderResult.Ok)
        {
            result.AddError(askOrderResult.Error);
            return result;
        }
        
        var bidOrderResult = await _repository.GetByIdAsync(messageValue.BidOrder.OrderId);
        if (!bidOrderResult.Ok)
        {
            result.AddError(bidOrderResult.Error);
            return result;
        }

        var askOrder = askOrderResult.Result;
        var bidOrder = bidOrderResult.Result;

        if (messageValue.AskOrder.Volume < messageValue.BidOrder.Volume)
        {
            askOrder.Status = OrderStatus.Executed;
            await _repository.UpdateAsync(askOrder);
            _logger.LogInformation($"Change order {askOrder.Id} status to Executed");
        }
        else if (messageValue.AskOrder.Volume > messageValue.BidOrder.Volume)
        {
            bidOrder.Status = OrderStatus.Executed;
            await _repository.UpdateAsync(bidOrder);
            _logger.LogInformation($"Change order {bidOrder.Id} status to Executed");
        }
        else
        {
            askOrder.Status = OrderStatus.Executed;
            bidOrder.Status = OrderStatus.Executed;
            
            await _repository.UpdateAsync(askOrder);
            await _repository.UpdateAsync(bidOrder);
            
            _logger.LogInformation($"Change order {askOrder.Id} status to Executed");
            _logger.LogInformation($"Change order {bidOrder.Id} status to Executed");
        }

        await _eventProducer.ProduceAsync(null, new OrderExecuteEvent()
        {
            AskInvestorId = askOrder.InvestorId,
            BidInvestorId = bidOrder.InvestorId,
            Price = askOrder.Price,
            Volume = askOrder.Volume < bidOrder.Volume ? askOrder.Volume : bidOrder.Volume,
            ProductId = askOrder.ProductId
        });

        return result;
    }
}