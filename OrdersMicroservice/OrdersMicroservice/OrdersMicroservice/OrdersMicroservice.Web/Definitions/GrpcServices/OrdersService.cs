using AutoMapper;
using Confluent.Kafka;
using Grpc.Core;
using Orders;
using OrdersEvent;
using OrdersMicroservice.Definitions.DepthMarket.Services;
using OrdersMicroservice.Definitions.Mongodb.Models;
using OrdersMicroservice.Domain.DbBase;
using OrdersMicroservice.EventsBase;

namespace OrdersMicroservice.Definitions.GrpcServices;

public class OrdersService : Orders.OrdersService.OrdersServiceBase
{
    private readonly IRepository<OrderModel> _repository;
    private readonly IMapper _mapper;
    private readonly IEventProducer<Null, OrderCreatedEvent> _eventProducer;
    private readonly DepthMarketSearchService _depthMarketSearchService;

    public OrdersService(
        IRepository<OrderModel> repository, 
        IMapper mapper, 
        IEventProducer<Null, OrderCreatedEvent> eventProducer,
        DepthMarketSearchService depthMarketSearchService)
    {
        _repository = repository;
        _mapper = mapper;
        _eventProducer = eventProducer;
        _depthMarketSearchService = depthMarketSearchService;
    }

    public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        var order = _mapper.Map<OrderModel>(request.OrderDetail);
        var hasAsk = await _depthMarketSearchService.HasAskAsync(order.ProductId);
        
        order.Status = hasAsk || order.OrderType == OrderTypes.Ask ? OrderStatus.Validating : OrderStatus.Aborted;
        var id = await _repository.AddAsync(order);

        if (!hasAsk && order.OrderType == OrderTypes.Bid)
        {
            return new CreateOrderResponse()
            {
                Error = new Orders.Error()
                {
                    ErrorText = "Order aborted"
                }
            };
        }

        await _eventProducer.ProduceAsync(null, new OrderCreatedEvent()
        {
            Order = request.OrderDetail,
            OrderId = id.Result
        });

        return new CreateOrderResponse()
        {
            Success = new Success()
            {
                SuccessText = "Order is processing"
            }
        };
    }
}