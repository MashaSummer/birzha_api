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
    private readonly IRepository<OrderModel> _repository;
    private readonly IMapper _mapper;
    private readonly IEventProducer<Null, OrderCreatedEvent> _eventProducer;

    public OrdersService(IRepository<OrderModel> repository, IMapper mapper, IEventProducer<Null, OrderCreatedEvent> eventProducer)
    {
        _repository = repository;
        _mapper = mapper;
        _eventProducer = eventProducer;
    }

    public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        var order = _mapper.Map<OrderModel>(request.OrderDetail);
        order.Status = OrderStatus.Validating;
        var id = await _repository.AddAsync(order);

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