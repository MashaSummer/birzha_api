using AutoMapper;
using Confluent.Kafka;
using Grpc.Core;
using Orders;
using OrdersEvent;
using OrdersMicroservice.Definitions.Mongodb.Models;
using OrdersMicroservice.Domain.DbBase;
using OrdersMicroservice.EventsBase;

namespace OrdersMicroservice.Definitions.GrpcServices;

public class OrdersService : Orders.OrdersService.OrdersServiceBase
{
    private readonly IDbWorker<OrderModel> _dbWorker;
    private readonly IMapper _mapper;
    private readonly IEventProducer<Null, OrderCreatedEvent> _eventProducer;

    public OrdersService(IDbWorker<OrderModel> dbWorker, IMapper mapper, IEventProducer<Null, OrderCreatedEvent> eventProducer)
    {
        _dbWorker = dbWorker;
        _mapper = mapper;
        _eventProducer = eventProducer;
    }

    public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        // TODO send request to validate
        
        // TODO move to kafka handler
        
        var order = _mapper.Map<OrderModel>(request.OrderDetail);
        order.Status = OrderStatus.Validating;
        await _dbWorker.AddNewRecord(order);

        await _eventProducer.ProduceAsync(null, new OrderCreatedEvent()
        {
            Order = request.OrderDetail,
            OrderId = "1111"
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